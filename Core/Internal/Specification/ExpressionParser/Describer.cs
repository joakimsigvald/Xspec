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

    /// Render Xspec's <c>A&lt;T&gt;</c> / <c>An&lt;T&gt;</c> / <c>The&lt;T&gt;</c>
    /// factory shapes. Three sub-cases: plain mention, with constraints, or
    /// with member-access drilldown. Detection lives on <see cref="Expr.AsMention"/>;
    /// this method shapes the result into text.
    protected bool TryDescribeMention(Expr expr, out string description)
    {
        description = string.Empty;
        if (expr.AsMention() is not { } m) return false;

        string head = $"{m.Verb.AsWords()} {m.TypeArgs}";

        if (m.Constraints is { Count: > 0 })
        {
            description = $"{head} {{ {DescribeAll(m.Constraints)} }}";
            return true;
        }
        if (!ReferenceEquals(m.Root, expr)
            && expr.Raw.Length > m.Root.Raw.Length
            && expr.Raw.StartsWith(m.Root.Raw))
        {
            string suffix = expr.Raw[m.Root.Raw.Length..].TrimStart();
            if (!suffix.StartsWith('.')) return false;
            description = $"{head}'s {suffix[1..]}";
            return true;
        }
        description = head;
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
