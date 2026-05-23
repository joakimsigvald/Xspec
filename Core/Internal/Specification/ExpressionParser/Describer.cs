using Xspec.Internal.Specification;

namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Base for the three description modes. Subclasses override
/// <see cref="Describe"/> for their mode-specific rendering, and inherit
/// Mention detection and <c>new</c>-expression rendering — both of which
/// always describe sub-expressions in value mode via <see cref="Value"/>.
/// </summary>
internal abstract class Describer
{
    /// Singleton value-mode describer, used wherever a sub-expression should
    /// be described as a value regardless of the outer context.
    public static readonly ValueDescriber Value = new();

    public abstract string Describe(Expr expr);

    protected string DescribeAll(IEnumerable<Expr> exprs) =>
        string.Join(", ", exprs.Select(Value.Describe));

    /// Recognize Xspec's <c>A&lt;T&gt;</c> / <c>An&lt;T&gt;</c> /
    /// <c>The&lt;T&gt;</c> factories. Three sub-shapes:
    /// plain mention, with constraints, or with member-access drilldown.
    protected bool TryDescribeMention(Expr expr, out string description)
    {
        description = string.Empty;
        if (!TryGetMentionRoot(expr, out var root, out var verb, out var typeArgs, out var constraints))
            return false;

        string head = $"{verb.AsWords()} {typeArgs}";

        if (constraints is { Count: > 0 })
        {
            description = $"{head} {{ {DescribeAll(constraints)} }}";
            return true;
        }
        if (!ReferenceEquals(root, expr)
            && expr.Raw.Length > root.Raw.Length
            && expr.Raw.StartsWith(root.Raw))
        {
            string suffix = expr.Raw[root.Raw.Length..].TrimStart();
            if (!suffix.StartsWith('.')) return false;
            description = $"{head}'s {suffix[1..]}";
            return true;
        }
        description = head;
        return true;
    }

    private static bool TryGetMentionRoot(
        Expr expr, out Expr root, out string verb, out string typeArgs,
        out IReadOnlyList<Expr>? constraints)
    {
        root = expr; verb = string.Empty; typeArgs = string.Empty; constraints = null;

        Expr cur = expr;
        while (cur is Call or Member or Index)
            cur = cur switch { Call c => c.Target, Member m => m.Target, Index i => i.Target, _ => cur };
        if (cur is not Generic g || g.Target is not Identifier id || g.TypeArgs.Count == 0)
            return false;

        verb = id.Name;
        typeArgs = string.Join(", ", g.TypeArgs.Select(t => t.Raw));

        // If the Generic sits directly inside a Call, that Call is the mention root;
        // its args (if any) are constraints.
        Expr cursor = expr;
        while (!ReferenceEquals(cursor, g))
        {
            if (cursor is Call cc && ReferenceEquals(cc.Target, g))
            {
                root = cc;
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
        root = g;
        return true;
    }

    /// Render a <c>new</c> expression. With an init block, the user-written
    /// prefix is preserved verbatim (covers <c>new T()</c>, <c>new int[]</c>,
    /// <c>new T&lt;U&gt;()</c>, etc.); inits are always value-described.
    protected string DescribeNew(New n)
    {
        var name = n.TypeName ?? string.Empty;
        if (n.Init is null)
        {
            var prefix = string.IsNullOrEmpty(name) ? "new" : $"new {name}";
            return $"{prefix}({DescribeAll(n.Args)})";
        }
        var initText = DescribeAll(n.Init);
        int braceIdx = n.Raw.IndexOf('{');
        if (braceIdx > 0) return $"{n.Raw[..braceIdx].TrimEnd()} {{ {initText} }}";
        var initPrefix = string.IsNullOrEmpty(name) ? "new" : $"new {name}";
        return n.Args.Count == 0
            ? $"{initPrefix} {{ {initText} }}"
            : $"{initPrefix}({DescribeAll(n.Args)}) {{ {initText} }}";
    }
}
