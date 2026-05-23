#pragma warning disable CS1591 // Public surface used internally; no XML docs required.

using System.Text;
using System.Text.RegularExpressions;
using Xspec.Internal.Specification.ExpressionParsing.Describe;
using Xspec.Internal.Specification.ExpressionParsing.Parse;

namespace Xspec.Internal.Specification;

public static partial class ExpressionParser
{
    private static readonly char[] _lineBreakCues = ['.', '(', '['];

    public static string ParseValue(this string? expr)
        => string.IsNullOrWhiteSpace(expr) ? string.Empty
        : Describer.Value.Describe(Parser.Parse(expr.ToSingleLine()));

    public static string? ParseCall(this string expr, bool skipSubjectRef = false)
        => expr is null ? null
        : string.IsNullOrWhiteSpace(expr) ? string.Empty
        : new CallDescriber(skipSubjectRef).Describe(Parser.Parse(expr.ToSingleLine()));

    public static string ParseActual(this string expr)
        => string.IsNullOrWhiteSpace(expr) ? string.Empty
        : new ActualDescriber().Describe(Parser.Parse(expr.ToSingleLine()));

    public static string ToSingleLine(this string str)
        => string.IsNullOrEmpty(str) ? str : MergeLines([.. ToLines(str)]);

    private static IEnumerable<string> ToLines(string str)
        => LineBreakRegex()
        .Split(str)
        .Select(StripLineComment)
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(s => s.Trim());

    private static string MergeLines(this string[] lines)
    {
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

    private static string StripLineComment(string line)
    {
        for (var i = 0; i < line.Length; i = Advance(line, i))
            if (line[i..].StartsWith("//"))
                return line[..i];
        return line;
    }

    /// Skips past any string or char literal at <paramref name="i"/>. If
    /// there's no literal here, advances one character.
    private static int Advance(string line, int i)
    {
        if (line[i] == '\'')
            return SkipEscapedLiteral(line, i + 1, close: '\'');

        var (openQuote, verbatim) = FindStringOpen(line, i);
        if (openQuote < 0) return i + 1;
        return verbatim
            ? SkipVerbatimString(line, openQuote + 1)
            : SkipEscapedLiteral(line, openQuote + 1, close: '"');
    }

    /// Locates the opening <c>"</c> of a string at position <paramref name="i"/>,
    /// optionally preceded by <c>@</c>/<c>$</c> prefix characters. Returns
    /// (-1, false) if there's no string here.
    private static (int OpenQuote, bool Verbatim) FindStringOpen(string line, int i)
    {
        int p = i;
        bool verbatim = false;
        while (p < line.Length && line[p] is '@' or '$')
        {
            if (line[p] == '@') verbatim = true;
            p++;
        }
        return p < line.Length && line[p] == '"' ? (p, verbatim) : (-1, false);
    }

    /// Returns the position past the closing <paramref name="close"/>,
    /// treating <c>\</c> as a two-char escape (regular strings, char literals).
    private static int SkipEscapedLiteral(string line, int from, char close)
    {
        int i = from;
        while (i < line.Length)
        {
            if (line[i] == '\\') { i = Math.Min(i + 2, line.Length); continue; }
            if (line[i] == close) return i + 1;
            i++;
        }
        return line.Length;
    }

    /// Returns the position past the closing <c>"</c>, treating <c>""</c> as
    /// the in-string escape (verbatim string rule).
    private static int SkipVerbatimString(string line, int from)
    {
        int i = from;
        while (i < line.Length)
        {
            if (line[i] != '"') { i++; continue; }
            if (i + 1 < line.Length && line[i + 1] == '"') { i += 2; continue; }
            return i + 1;
        }
        return line.Length;
    }

    [GeneratedRegex(@"(\r|\n)+")]
    private static partial Regex LineBreakRegex();
}