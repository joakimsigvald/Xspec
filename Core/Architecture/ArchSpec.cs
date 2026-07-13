using System.Runtime.CompilerServices;
using Xspec.Internal.Specification;

namespace Xspec.Architecture;

/// <summary>
/// Base-class for specifying architecture rules over the solution model.
/// Each test method states one rule: when examining a set of elements,
/// then they satisfy some invariant.
/// </summary>
public abstract class ArchSpec : IDisposable
{
    private readonly SpecificationContext _specification;
    private readonly Lazy<string> _lazySpecification;

    /// <summary>
    /// Initialize the specification generation for the given solution model
    /// </summary>
    /// <param name="solution">The solution model that the rules are expressed over</param>
    protected ArchSpec(Solution solution)
    {
        Solution = solution;
        _specification = SpecificationContext.Create();
        _lazySpecification = new(_specification.ToString);
    }

    /// <summary>
    /// The solution model that the rules are expressed over
    /// </summary>
    protected Solution Solution { get; }

    /// <summary>
    /// All projects of the solution
    /// </summary>
    protected IReadOnlyList<Project> Projects => Solution.Projects;

    /// <summary>
    /// This property returns the specification of the rule, after the rule has been verified.
    /// The specification will also be included in the message of the exception if the rule fails.
    /// Assertions on the specification are line-ending-agnostic.
    /// </summary>
    public SpecificationText Specification => new(_lazySpecification);

    /// <summary>
    /// Define the set of elements that the rule examines
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <param name="items">The elements that the rule examines</param>
    /// <param name="that">An optional condition narrowing the set to the elements subject to the rule</param>
    /// <param name="itemsExpr">Captured automatically by the compiler — do not provide</param>
    /// <param name="thatExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for stating the rule</returns>
    protected ArchScope<T> When<T>(
        IReadOnlyCollection<T> items,
        Func<T, bool>? that = null,
        [CallerArgumentExpression(nameof(items))] string? itemsExpr = null,
        [CallerArgumentExpression(nameof(that))] string? thatExpr = null)
    {
        var itemsText = itemsExpr!.ParseValue();
        var predicate = that is null ? null : thatExpr.DescribePredicate();
        // The When-line reads as a clause ("Projects are contracts") while the failure
        // message needs the scope as a noun phrase ("Projects that are contracts").
        _specification.AddArchWhen(predicate is null ? itemsText : $"{itemsText} {predicate}");
        return new(
            _specification,
            that is null ? items : [.. items.Where(that)],
            predicate is null ? itemsText : $"{itemsText} that {predicate}");
    }

    /// <summary>
    /// Releases the specification context of the rule
    /// </summary>
    public void Dispose() => SpecificationContext.Release();
}
