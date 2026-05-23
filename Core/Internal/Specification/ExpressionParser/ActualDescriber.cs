namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Describes an expression in "actual" context (used by <c>ParseActual</c>).
/// Walks the rightmost member-access chain to find the wrapping
/// <c>Then(...)</c> or <c>And(...)</c> call, then renders just the tail
/// after that wrapper. Enforces the "no trainwrecks in And" rule.
/// </summary>
internal static class ActualDescriber
{
    public static string Describe(string source, Expr expr)
    {
        if (string.IsNullOrEmpty(source)) return string.Empty;
        if (source.EndsWith(".That", StringComparison.InvariantCultureIgnoreCase)) return string.Empty;

        var tail = new List<string>();
        Expr cur = expr;
        while (true)
        {
            if (cur is Member m) { tail.Insert(0, m.Name); cur = m.Target; continue; }
            if (cur is not Call c) break;

            string? methodName = ShapeRenderer.GetMethodName(c);
            if (methodName is "Then" or "And")
            {
                if (methodName == "And" && c.Args.Any(ContainsMember))
                    throw new SetupFailed("No trainwrecks in And! chain additional properties/method calls outside of the And-expression");
                return CombinePrefixTail(c.Args.Count >= 1 ? ValueDescriber.Describe(c.Args[0]) : "", tail);
            }
            if (c.Target is Member memCall)
            {
                tail.Insert(0, $"{memCall.Name}({string.Join(", ", c.Args.Select(a => a.Raw))})");
                cur = memCall.Target;
                continue;
            }
            break;
        }

        if (tail.Count == 0) return ValueDescriber.Describe(expr);
        var baseStr = cur is Identifier ii ? ii.Name : cur.Raw;
        return string.Join('.', new[] { baseStr }.Concat(tail));
    }

    private static string CombinePrefixTail(string prefix, List<string> tail)
    {
        if (tail.Count == 0) return prefix;
        if (string.IsNullOrEmpty(prefix)) return string.Join('.', tail);
        return IsOneWord(prefix)
            ? $"{prefix}.{string.Join('.', tail)}"
            : $"{prefix}'s {string.Join('.', tail)}";
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
        _ => false,
    };
}
