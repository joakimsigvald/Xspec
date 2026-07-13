using System.Text.RegularExpressions;

namespace Xspec.Internal.Specification;

/// <summary>
/// Renders architecture-rule lambdas as specification text. Simple named predicates
/// (<c>_ =&gt; _.IsContract()</c>) are rendered as plural words ("are contracts");
/// anything more complex stays code-shaped, so complex rules read best when
/// factored into named extension methods.
/// </summary>
internal static partial class ArchDescriber
{
    internal static string DescribePredicate(this string? filterExpr)
    {
        var text = StripSubjectRefs(filterExpr);
        var negated = text.StartsWith('!');
        if (negated)
            text = text[1..];
        if (SingleIdentifier().IsMatch(text))
            return Verbalize(text.TrimEnd('(', ')').ToWords(), negated);
        text = text.Replace(" || ", " or ").Replace(" && ", " and ");
        return negated ? $"not {text}" : text;
    }

    internal static string DescribeSelector(this string? selectExpr) => StripSubjectRefs(selectExpr);

    private static string Verbalize(string[] words, bool negated)
    {
        var pluralVerb = words[0] switch
        {
            "is" => "are",
            "has" => "have",
            "does" => "do",
            _ => null,
        };
        if (pluralVerb is not null && words.Length > 1)
        {
            words[0] = pluralVerb;
            words[^1] = Pluralize(words[^1]);
            if (negated)
                return string.Join(' ', words.Take(1).Append("not").Concat(words.Skip(1)));
        }
        var phrase = string.Join(' ', words);
        return negated ? $"not {phrase}" : phrase;
    }

    private static string Pluralize(string word)
        => word.EndsWith('y') ? $"{word[..^1]}ies"
        : word.EndsWith('s') || word.EndsWith('x') || word.EndsWith("ch") || word.EndsWith("sh") ? $"{word}es"
        : $"{word}s";

    private static string StripSubjectRefs(string? lambdaExpr)
        => SubjectRef().Replace(lambdaExpr.ParseCall(true) ?? string.Empty, string.Empty);

    [GeneratedRegex(@"(?<![\w.])_\.")]
    private static partial Regex SubjectRef();

    [GeneratedRegex(@"^\w+(\(\))?$")]
    private static partial Regex SingleIdentifier();
}
