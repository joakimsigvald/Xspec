namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Small utilities shared across the describer modes. Lambda-shape predicates
/// (<see cref="IsParamRef"/>, <see cref="IsParamRefAssignment"/>), join/word
/// helpers, and trivial node-text renderers (literals, with-inits,
/// assignment targets).
/// </summary>
internal static class DescribeHelpers
{
    public static string AsWords(string s) => Xspec.Internal.Specification.StringExtensions.AsWords(s);

    public static string JoinDescribed(IEnumerable<Expr> exprs)
        => string.Join(", ", exprs.Select(ValueDescriber.Describe));

    public static bool IsParamRef(IReadOnlyList<string> paramList, Expr e)
        => e is Identifier id && paramList.Count > 0
            && (id.Name == paramList[0] || paramList[0] == "_");

    public static bool IsParamRefAssignment(Lambda l, out string prop, out Expr value, out string op)
    {
        prop = string.Empty;
        value = l.Body;
        op = string.Empty;
        if (l.Body is Assign a && a.Target is Member m && IsParamRef(l.Params, m.Target))
        {
            prop = m.Name;
            value = a.Value;
            op = a.Op;
            return true;
        }
        return false;
    }

    public static string DescribeWithInits(With w) => JoinDescribed(w.Init);

    public static string DescribeAssignTarget(Expr target) => target switch
    {
        Member m => m.Name,
        Identifier id => id.Name,
        _ => target.Raw,
    };

    public static string DescribeLiteral(string raw) => Requote(raw);

    public static string Requote(string raw)
    {
        int q = raw.IndexOf('"');
        if (q < 0 || raw.Length < 2 || raw[^1] != '"') return raw;
        return $"\"{raw[(q + 1)..^1]}\"";
    }
}
