namespace Xspec.Internal.Specification.ExpressionParserInternals;

internal static class Describer
{
    public static string DescribeValue(Expr expr) => expr switch
    {
        Lambda l when l.Params.Count <= 2 && l.Body is Assign a && IsParamRefAssignment(l.Params, a, out var prop, out var value, out var op) && op == "="
            => $"{prop} = {DescribeValue(value)}",
        Lambda l when l.Params.Count <= 1 && l.Body is With w => DescribeWithInits(w),
        Lambda l when l.Params.Count <= 1 => DescribeValue(l.Body),
        Lambda l => l.Raw,
        Assign a => $"{DescribeAssignTarget(a.Target)} {a.Op} {DescribeValue(a.Value)}",
        With w => DescribeWithInits(w),
        Tuple t => $"({string.Join(", ", t.Items.Select(DescribeValue))})",
        ArrayLit arr => $"[{string.Join(", ", arr.Items.Select(DescribeValue))}]",
        Binary b => $"{DescribeValue(b.Left)} {b.Op} {DescribeValue(b.Right)}",
        Unary u => $"{u.Op}{DescribeValue(u.Operand)}",
        Postfix p => $"{DescribeValue(p.Operand)}{p.Op}",
        Conditional c => $"{DescribeValue(c.Cond)} ? {DescribeValue(c.Then)} : {DescribeValue(c.Else)}",
        Cast c => $"({c.TypeName}){DescribeValue(c.Operand)}",
        IsAs ia => $"{DescribeValue(ia.Operand)} {ia.Op} {ia.TypeName}",
        InterpolatedString s => DescribeQuoted(s.Raw),
        Literal lit => DescribeLiteral(lit.Raw),
        New n => DescribeNew(n),
        Call call when call.Target is Literal litT && litT.Raw == "default" && call.Args.Count == 1
            => $"default {DescribeValue(call.Args[0])}",
        Call call when TryDescribeMention(call, out var m) => m,
        Generic g when TryDescribeMention(g, out var m) => m,
        Member memx when TryDescribeMention(memx, out var m) => m,
        Index idxx when TryDescribeMention(idxx, out var m) => m,
        Call call when call.Target is Identifier id && call.Args.Count >= 1
            => $"{Xspec.Internal.Specification.StringExtensions.AsWords(id.Name)} {string.Join(", ", call.Args.Select(DescribeValue))}",
        Call call => DescribeCallShape(call),
        Generic g => g.Raw,
        Member m => DescribeMember(m),
        Identifier id => id.Name,
        Unknown u => u.Raw,
        _ => expr.Raw,
    };

    public static string DescribeCall(Expr expr, bool skipSubjectRef = false)
    {
        if (expr is Lambda l)
        {
            if (l.Params.Count == 1
                && l.Body is Call mc && mc.Target is Member mem
                && IsParamRef(l.Params, mem.Target))
            {
                var argText = string.Join(", ", mc.Args.Select(DescribeValue));
                return skipSubjectRef
                    ? $"{mem.Name}({argText})"
                    : $"{((Identifier)mem.Target).Name}.{mem.Name}({argText})";
            }
            if (l.Params.Count == 1
                && l.Body is Assign asgn && asgn.Target is Member assMem
                && IsParamRef(l.Params, assMem.Target))
            {
                var paramName = ((Identifier)assMem.Target).Name;
                return skipSubjectRef
                    ? $"{assMem.Name} {asgn.Op} {DescribeValue(asgn.Value)}"
                    : $"{paramName}.{assMem.Name} {asgn.Op} {DescribeValue(asgn.Value)}";
            }
            if (l.Params.Count == 1 && skipSubjectRef
                && l.Body is Unknown u && u.Raw.StartsWith(l.Params[0] + "."))
            {
                return u.Raw[(l.Params[0].Length + 1)..];
            }
            if (l.Params.Count >= 2
                && l.Body is Assign asgn2 && asgn2.Target is Member assMem2
                && IsParamRef(l.Params, assMem2.Target) && asgn2.Op == "=")
            {
                return $"{assMem2.Name} = {DescribeValue(asgn2.Value)}";
            }
            if (l.Params.Count == 1)
                return DescribeValue(l.Body);
            return l.Raw;
        }
        return expr switch
        {
            New n => DescribeNew(n),
            Call call when TryDescribeMention(call, out var m) => m,
            Generic g when TryDescribeMention(g, out var m) => m,
            Call call => DescribeCallShape(call),
            _ => DescribeValue(expr),
        };
    }

    public static string DescribeActual(string source, Expr expr)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        if (source.EndsWith(".That", StringComparison.InvariantCultureIgnoreCase))
            return string.Empty;

        var tail = new List<string>();
        Expr cur = expr;
        while (true)
        {
            if (cur is Member m)
            {
                tail.Insert(0, m.Name);
                cur = m.Target;
                continue;
            }
            if (cur is Call c)
            {
                string? methodName = GetMethodName(c);
                if (methodName is "Then" or "And")
                {
                    if (methodName == "And" && c.Args.Any(ContainsMember))
                        throw new SetupFailed("No trainwrecks in And! chain additional properties/method calls outside of the And-expression");

                    string prefix = c.Args.Count >= 1 ? DescribeValue(c.Args[0]) : string.Empty;
                    if (tail.Count == 0)
                        return prefix;
                    if (string.IsNullOrEmpty(prefix))
                        return string.Join('.', tail);
                    return IsOneWord(prefix)
                        ? $"{prefix}.{string.Join('.', tail)}"
                        : $"{prefix}'s {string.Join('.', tail)}";
                }
                if (c.Target is Member memCall)
                {
                    tail.Insert(0, $"{memCall.Name}({string.Join(", ", c.Args.Select(a => a.Raw))})");
                    cur = memCall.Target;
                    continue;
                }
            }
            break;
        }

        if (tail.Count == 0)
            return DescribeValue(expr);

        var baseStr = cur switch
        {
            Identifier ii => ii.Name,
            _ => cur.Raw,
        };
        return string.Join('.', new[] { baseStr }.Concat(tail));
    }

    private static string? GetMethodName(Call c) => c.Target switch
    {
        Identifier id => id.Name,
        Member m => m.Name,
        Generic g when g.Target is Identifier gi => gi.Name,
        Generic g when g.Target is Member gm => gm.Name,
        _ => null,
    };

    private static string DescribeCallShape(Call call)
    {
        var argText = string.Join(", ", call.Args.Select(DescribeValue));
        return call.Target switch
        {
            Generic g => $"{DescribeCallTarget(g)}({argText})",
            Member m => $"{DescribeMemberTarget(m.Target)}.{m.Name}({argText})",
            Identifier id => $"{id.Name}({argText})",
            _ => $"{call.Target.Raw}({argText})",
        };
    }

    private static string DescribeCallTarget(Expr target) => target switch
    {
        Generic g when g.Target is Identifier id
            => $"{id.Name}<{string.Join(", ", g.TypeArgs.Select(t => t.Raw))}>",
        Member m => $"{DescribeMemberTarget(m.Target)}.{m.Name}",
        Identifier id => id.Name,
        _ => target.Raw,
    };

    private static string DescribeMemberTarget(Expr target) => target switch
    {
        Identifier id => id.Name,
        Member m => $"{DescribeMemberTarget(m.Target)}.{m.Name}",
        _ => target.Raw,
    };

    private static string DescribeMember(Member m) => $"{DescribeMemberTarget(m.Target)}.{m.Name}";

    private static string DescribeAssignTarget(Expr target) => target switch
    {
        Member m => m.Name,
        Identifier id => id.Name,
        _ => target.Raw,
    };

    private static bool IsParamRef(IReadOnlyList<string> paramList, Expr e)
        => e is Identifier id && paramList.Count > 0
            && (id.Name == paramList[0] || paramList[0] == "_");

    private static bool IsParamRefAssignment(IReadOnlyList<string> paramList, Assign a, out string prop, out Expr value, out string op)
    {
        prop = string.Empty;
        value = a.Value;
        op = a.Op;
        if (a.Target is Member m && IsParamRef(paramList, m.Target))
        {
            prop = m.Name;
            return true;
        }
        return false;
    }

    private static string DescribeWithInits(With w)
        => string.Join(", ", w.Init.Select(DescribeValue));

    private static string DescribeNew(New n)
    {
        var argText = string.Join(", ", n.Args.Select(DescribeValue));
        var name = n.TypeName ?? string.Empty;
        if (n.Init is null)
        {
            var prefix = string.IsNullOrEmpty(name) ? "new" : $"new {name}";
            return $"{prefix}({argText})";
        }
        var initText = string.Join(", ", n.Init.Select(DescribeValue));
        var preBracePrefix = ExtractNewPrefix(n);
        if (preBracePrefix is not null)
            return $"{preBracePrefix} {{ {initText} }}";
        var initPrefix = string.IsNullOrEmpty(name) ? "new" : $"new {name}";
        if (n.Args.Count == 0)
            return $"{initPrefix} {{ {initText} }}";
        return $"{initPrefix}({argText}) {{ {initText} }}";
    }

    private static string? ExtractNewPrefix(New n)
    {
        int braceIdx = n.Raw.IndexOf('{');
        if (braceIdx <= 0) return null;
        return n.Raw[..braceIdx].TrimEnd();
    }

    private static string DescribeLiteral(string raw)
    {
        if (raw.Length >= 2 && raw[^1] == '"')
        {
            int quoteStart = raw.IndexOf('"');
            if (quoteStart >= 0)
                return $"\"{raw[(quoteStart + 1)..^1]}\"";
        }
        return raw;
    }

    private static string DescribeQuoted(string raw)
    {
        int quoteStart = raw.IndexOf('"');
        if (quoteStart < 0 || raw.Length < 2 || raw[^1] != '"')
            return raw;
        return $"\"{raw[(quoteStart + 1)..^1]}\"";
    }

    private static bool TryDescribeMention(Expr expr, out string description)
    {
        description = string.Empty;
        if (!TryGetMentionRoot(expr, out var mentionRoot, out var verb, out var typeArgsRaw, out var constraints))
            return false;

        var verbWords = Xspec.Internal.Specification.StringExtensions.AsWords(verb);
        string head = $"{verbWords} {typeArgsRaw}";

        if (constraints is not null && constraints.Count > 0)
        {
            var inner = string.Join(", ", constraints.Select(DescribeValue));
            description = $"{head} {{ {inner} }}";
            return true;
        }

        string mentionRaw = mentionRoot.Raw;
        string outerRaw = expr.Raw;
        if (!ReferenceEquals(mentionRoot, expr) && outerRaw.Length > mentionRaw.Length && outerRaw.StartsWith(mentionRaw))
        {
            string suffix = outerRaw[mentionRaw.Length..].TrimStart();
            if (!suffix.StartsWith('.'))
                return false;
            string tail = suffix[1..];
            description = $"{head}'s {tail}";
            return true;
        }

        description = head;
        return true;
    }

    private static bool TryGetMentionRoot(
        Expr expr,
        out Expr mentionRoot,
        out string verb,
        out string typeArgsRaw,
        out IReadOnlyList<Expr>? constraints)
    {
        mentionRoot = expr;
        verb = string.Empty;
        typeArgsRaw = string.Empty;
        constraints = null;

        Expr root = expr;
        while (true)
        {
            switch (root)
            {
                case Call c: root = c.Target; continue;
                case Member m: root = m.Target; continue;
                case Index i: root = i.Target; continue;
            }
            break;
        }

        if (root is not Generic g) return false;
        if (g.Target is not Identifier id) return false;
        if (g.TypeArgs.Count == 0) return false;

        verb = id.Name;
        typeArgsRaw = string.Join(", ", g.TypeArgs.Select(t => t.Raw));

        Expr cursor = expr;
        Call? wrappingCall = null;
        while (true)
        {
            if (cursor is Call cc && ReferenceEquals(cc.Target, g))
            {
                wrappingCall = cc;
                break;
            }
            if (ReferenceEquals(cursor, g)) break;
            cursor = cursor switch
            {
                Call cc2 => cc2.Target,
                Member mm => mm.Target,
                Index ii => ii.Target,
                _ => g,
            };
        }

        if (wrappingCall is { } w)
        {
            mentionRoot = w;
            if (w.Args.Count > 0)
                constraints = w.Args;
        }
        else
        {
            mentionRoot = g;
        }
        return true;
    }

    private static bool IsOneWord(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (var c in s)
            if (!(char.IsLetterOrDigit(c) || c is '_' or '(' or ')' or '?' or '!' or '.' or '<' or '>'))
                return false;
        return true;
    }

    private static bool ContainsMember(Expr e) => e switch
    {
        Member => true,
        Call c => c.Args.Any(ContainsMember) || ContainsMember(c.Target),
        Generic g => g.TypeArgs.Any(ContainsMember) || ContainsMember(g.Target),
        Index i => i.Args.Any(ContainsMember) || ContainsMember(i.Target),
        New n => n.Args.Any(ContainsMember) || (n.Init?.Any(ContainsMember) ?? false),
        With w => w.Init.Any(ContainsMember) || ContainsMember(w.Target),
        Lambda l => ContainsMember(l.Body),
        Assign a => ContainsMember(a.Target) || ContainsMember(a.Value),
        Binary b => ContainsMember(b.Left) || ContainsMember(b.Right),
        Unary u => ContainsMember(u.Operand),
        Postfix p => ContainsMember(p.Operand),
        Conditional c => ContainsMember(c.Cond) || ContainsMember(c.Then) || ContainsMember(c.Else),
        Tuple t => t.Items.Any(ContainsMember),
        ArrayLit a => a.Items.Any(ContainsMember),
        Cast c => ContainsMember(c.Operand),
        IsAs ia => ContainsMember(ia.Operand),
        InterpolatedString => false,
        _ => false,
    };
}
