using System.Text;
using System.Text.RegularExpressions;
using Xspec.Internal.Specification.ExpressionParserInternals;

namespace Xspec.Internal.Specification;

/// <summary>
///
/// </summary>
public static partial class ExpressionParser
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static string ParseValue(this string? expr)
    {
        if (string.IsNullOrWhiteSpace(expr)) return string.Empty;
        var source = expr.ToSingleLine();
        if (string.IsNullOrEmpty(source)) return string.Empty;
        return Describer.Value.Describe(Parser.Parse(source));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="expr"></param>
    /// <param name="skipSubjectRef"></param>
    /// <returns></returns>
    public static string ParseCall(this string expr, bool skipSubjectRef = false)
    {
        var source = expr.ToSingleLine();
        if (string.IsNullOrEmpty(source)) return source;
        return new CallDescriber(skipSubjectRef).Describe(Parser.Parse(source));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static string ParseActual(this string expr)
    {
        var source = expr.ToSingleLine();
        if (string.IsNullOrEmpty(source)) return string.Empty;
        return new ActualDescriber(source).Describe(Parser.Parse(source));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSingleLine(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        var lines = LineBreakRegex()
            .Split(str)
            .Select(StripLineComment)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim());
        StringBuilder sb = new();
        bool addSpace = false;
        foreach (var line in lines)
        {
            if (addSpace && !line.StartsWith('.'))
                sb.Append(' ');
            sb.Append(line);
            addSpace = !_lineBreakCues.Contains(line[^1]);
        }
        return sb.ToString();
    }

    private static readonly char[] _lineBreakCues = ['.', '(', '['];

    private static string StripLineComment(string line)
    {
        int idx = line.IndexOf("//");
        if (idx < 0) return line;
        bool inString = false;
        for (int i = 0; i < idx; i++)
        {
            if (line[i] == '"' && (i == 0 || line[i - 1] != '\\')) inString = !inString;
        }
        return inString ? line : line[..idx];
    }

    [GeneratedRegex(@"(\r|\n)+")]
    private static partial Regex LineBreakRegex();
}
