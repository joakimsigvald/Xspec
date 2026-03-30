using System.Runtime.CompilerServices;
using Xspec.Internal.Specification;

namespace Xspec.Assert.Continuations.Enumerable;

/// <summary>
/// Continuation that allows an assertions to be made on the provided enumerable
/// </summary>
/// <typeparam name="TItem"></typeparam>
public record DoesEnumerable<TItem> : EnumerableConstraint<TItem, DoesEnumerableContinuation<TItem>>
{
    /// <summary>
    /// Assert that the enumerable contains the given item
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for making additional asserts on the enumerable</returns>
    public ContinueWith<DoesEnumerableContinuation<TItem>> Contain(
        TItem expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert($"{expected}",
            NotNullAnd(actual => Xunit.Assert.Contains(expected, actual)),
            expectedExpr!, verbalizationStrategy: VerbalizationStrategy.PresentSingularS).And();

    internal override DoesEnumerableContinuation<TItem> Continue() => Create(Actual, ActualExpr);
}