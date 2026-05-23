namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Renders <c>new</c> expressions. When an init block is present, preserves
/// the user-written prefix verbatim (covers <c>new T</c>, <c>new T()</c>,
/// <c>new int[]</c>, <c>new T&lt;U&gt;()</c>, target-typed <c>new(...)</c>).
/// </summary>
internal static class NewDescriber
{
    public static string Describe(New n)
    {
        var name = n.TypeName ?? string.Empty;
        if (n.Init is null)
        {
            var prefix = string.IsNullOrEmpty(name) ? "new" : $"new {name}";
            return $"{prefix}({DescribeHelpers.JoinDescribed(n.Args)})";
        }

        var initText = DescribeHelpers.JoinDescribed(n.Init);
        int braceIdx = n.Raw.IndexOf('{');
        if (braceIdx > 0) return $"{n.Raw[..braceIdx].TrimEnd()} {{ {initText} }}";

        var initPrefix = string.IsNullOrEmpty(name) ? "new" : $"new {name}";
        return n.Args.Count == 0
            ? $"{initPrefix} {{ {initText} }}"
            : $"{initPrefix}({DescribeHelpers.JoinDescribed(n.Args)}) {{ {initText} }}";
    }
}
