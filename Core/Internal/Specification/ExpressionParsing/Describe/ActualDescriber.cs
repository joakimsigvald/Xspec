using Xspec.Internal.Specification.ExpressionParsing.Expressions;

namespace Xspec.Internal.Specification.ExpressionParsing.Describe;

/// <summary>
/// Actual-mode description (used by <c>ParseActual</c>). Walks the rightmost
/// member-access chain to find the wrapping <c>Then(...)</c> / <c>And(...)</c>
/// call, then returns just the tail after that wrapper. Enforces the
/// "no trainwrecks in And" rule.
/// </summary>
internal sealed class ActualDescriber : Describer
{
    private readonly string _source;

    public ActualDescriber(string source) { _source = source; }

    public override string Describe(Expr expr)
    {
        if (string.IsNullOrEmpty(_source)) return string.Empty;
        if (_source.EndsWith(".That", StringComparison.InvariantCultureIgnoreCase)) return string.Empty;

        var tail = new List<string>();
        Expr cur = expr;
        while (true)
        {
            if (cur is Member m) { tail.Insert(0, m.Name); cur = m.Target; continue; }
            if (cur is not Call c) break;

            if (c.MethodName is "Then" or "And")
            {
                if (c.MethodName == "And" && c.Args.Any(ContainsMember))
                    throw new SetupFailed("No trainwrecks in And! chain additional properties/method calls outside of the And-expression");
                return Combine(c.Args.Count >= 1 ? Value.Describe(c.Args[0]) : "", tail);
            }
            if (c.Target is Member memCall)
            {
                tail.Insert(0, $"{memCall.Name}({string.Join(", ", c.Args.Select(a => a.Raw))})");
                cur = memCall.Target;
                continue;
            }
            break;
        }

        if (tail.Count == 0) return Value.Describe(expr);
        var baseStr = cur is Identifier ii ? ii.Name : cur.Raw;
        return string.Join('.', new[] { baseStr }.Concat(tail));
    }

    private static string Combine(string prefix, List<string> tail)
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

    private static bool ContainsMember(Expr e) =>
        e is Member || e.Children.Any(ContainsMember);
}