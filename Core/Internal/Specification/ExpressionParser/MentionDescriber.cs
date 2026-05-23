namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Detects and renders "Mention" expressions — Xspec's <c>A&lt;T&gt;()</c> /
/// <c>An&lt;T&gt;()</c> / <c>The&lt;T&gt;()</c> / etc. factory calls. Handles
/// three sub-shapes:
/// <list type="bullet">
///   <item>plain mention → "verb type"</item>
///   <item>with constraints → "verb type { describe(arg1), ... }"</item>
///   <item>with member-access drilldown → "verb type's tail"</item>
/// </list>
/// </summary>
internal static class MentionDescriber
{
    public static bool TryDescribe(Expr expr, out string description)
    {
        description = string.Empty;
        if (!TryGetMentionRoot(expr, out var mentionRoot, out var verb, out var typeArgsRaw, out var constraints))
            return false;

        string head = $"{DescribeHelpers.AsWords(verb)} {typeArgsRaw}";

        if (constraints is { Count: > 0 })
        {
            description = $"{head} {{ {DescribeHelpers.JoinDescribed(constraints)} }}";
            return true;
        }

        // Drilldown: mention sits at the START of the outer expression, followed by `.`
        if (!ReferenceEquals(mentionRoot, expr)
            && expr.Raw.Length > mentionRoot.Raw.Length
            && expr.Raw.StartsWith(mentionRoot.Raw))
        {
            string suffix = expr.Raw[mentionRoot.Raw.Length..].TrimStart();
            if (!suffix.StartsWith('.')) return false;
            description = $"{head}'s {suffix[1..]}";
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

        // Walk down through Call/Member/Index wrappers looking for the Generic at the root.
        Expr root = expr;
        while (root is Call or Member or Index)
            root = root switch { Call c => c.Target, Member m => m.Target, Index i => i.Target, _ => root };
        if (root is not Generic g || g.Target is not Identifier id || g.TypeArgs.Count == 0)
            return false;

        verb = id.Name;
        typeArgsRaw = string.Join(", ", g.TypeArgs.Select(t => t.Raw));

        // Find the Call (if any) wrapping the Generic directly
        Expr cursor = expr;
        while (!ReferenceEquals(cursor, g))
        {
            if (cursor is Call cc && ReferenceEquals(cc.Target, g))
            {
                mentionRoot = cc;
                if (cc.Args.Count > 0) constraints = cc.Args;
                return true;
            }
            cursor = cursor switch
            {
                Call cc2 => cc2.Target,
                Member mm => mm.Target,
                Index ii => ii.Target,
                _ => g,
            };
        }
        mentionRoot = g;
        return true;
    }
}
