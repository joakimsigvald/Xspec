using Xspec.Internal.Specification.ExpressionParsing.Expressions;

namespace Xspec.Internal.Specification.ExpressionParsing.Describe;

/// <summary>
/// Actual-mode description (used by <c>ParseActual</c>). Walks the rightmost
/// member-access chain to find the wrapping <c>Then(...)</c> / <c>And(...)</c>
/// call, then returns just the tail after that wrapper, prefixed by the
/// <paramref name="subject"/> the wrapper registered at runtime — the wrapper's
/// arguments are never interpreted here.
/// </summary>
internal sealed class ActualDescriber(string? subject = null) : Describer
{
    private const string _then = "Then";
    private const string _and = "And";
    private const string _that = "That";
    private const string _andBinder = "and";

    public override string Describe(Expr expr)
    {
        if (expr is Member top && string.Equals(top.Name, _that, StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        var tail = new List<(string Name, bool NullCond)>();
        Expr cur = expr;
        while (true)
        {
            if (cur is Member m)
            {
                // The and continuation property is a binding word, not part of the asserted value
                if (m.Name != _andBinder)
                    tail.Insert(0, (m.Name, m.NullConditional));
                cur = m.Target;
                continue;
            }
            if (cur is not Call c)
                break;

            if (c.MethodName is _then or _and)
                return Combine(subject ?? "", tail);

            if (c.Target is not Member memCall)
                break;

            var segment = $"{memCall.Name}({string.Join(", ", c.Args.Select(a => a.Raw))})";
            tail.Insert(0, (segment, memCall.NullConditional));
            cur = memCall.Target;
        }

        if (tail.Count == 0) 
            return Value.Describe(expr);

        var baseStr = cur is Identifier ii ? ii.Name : cur.Raw;
        return baseStr + Sep(tail[0]) + StitchBare(tail);
    }

    private static string Combine(string prefix, List<(string Name, bool NullCond)> tail)
    {
        if (tail.Count == 0) 
            return prefix;

        if (string.IsNullOrEmpty(prefix)) 
            return StitchBare(tail);

        return IsOneWord(prefix)
            ? prefix + Sep(tail[0]) + StitchBare(tail)
            : $"{prefix}'s {StitchBare(tail)}";
    }

    private static string Sep((string _, bool NullCond) seg) => seg.NullCond ? "?." : ".";

    private static string StitchBare(List<(string Name, bool NullCond)> tail)
    {
        var sb = new System.Text.StringBuilder(tail[0].Name);
        for (int i = 1; i < tail.Count; i++)
        {
            sb.Append(Sep(tail[i]));
            sb.Append(tail[i].Name);
        }
        return sb.ToString();
    }

    private static bool IsOneWord(string s) => !string.IsNullOrEmpty(s) && s.All(char.IsLetterOrDigit);
}