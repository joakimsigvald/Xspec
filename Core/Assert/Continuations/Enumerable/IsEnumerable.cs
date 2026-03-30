using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.Enumerable;

/// <summary>
/// Continuation that allows an assertions to be made on the provided enumerable
/// </summary>
/// <typeparam name="TItem"></typeparam>
public record IsEnumerable<TItem> : EnumerableConstraint<TItem, IsEnumerableContinuation<TItem>>
{
    /// <summary>
    /// Assert that the enumerable is empty
    /// </summary>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Empty()
        => Assert(Ignore.Me, NotNullAnd(Xunit.Assert.Empty)).And();

    /// <summary>
    /// Assert that all the elements of the enumerable are different from each other, using identity
    /// </summary>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Distinct()
        => Assert(Ignore.Me, NotNullAnd(Xunit.Assert.Distinct)).And();

    /// <summary>
    /// Assert that all the elements of the enumerable are different with respect to the given selector
    /// </summary>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Distinct<TSelector>(
        Func<TItem, TSelector> selector,
        [CallerArgumentExpression(nameof(selector))] string? selectorExpr = null)
    {
        var expectedStr = ExpressExpectation(selectorExpr!);
        return Assert(expectedStr, NotNullAnd(list => Xunit.Assert.Distinct(list.Select(selector))), expectedStr).And();
    }

    /// <summary>
    /// Assert that all the elements of the enumerable are different with respect to the given selector
    /// </summary>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Distinct<TSelector>(
        Func<TItem, int, TSelector> selector,
        [CallerArgumentExpression(nameof(selector))] string? selectorExpr = null)
    {
        var expectedStr = ExpressExpectation(selectorExpr!);
        return Assert(expectedStr, NotNullAnd(list => Xunit.Assert.Distinct(list.Select(selector))), expectedStr).And();
    }

    private string ExpressExpectation(string selectorStr) => $"by {selectorStr}";

    /// <summary>
    /// Assert that the enumerable is null
    /// </summary>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Null()
        => Assert(Ignore.Me, Xunit.Assert.Null).And();

    /// <summary>
    /// Assert that the two enumerables do not refer to the same object
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr">Provided by the compiler for building the specification description</param>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Not(
        IEnumerable<TItem> expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => not.SameAs(expected, expectedExpr);

    /// <summary>
    /// Assert that both enumerables has the same number of elements and that elements at same position are equal to each other
    /// </summary>
    /// <param name="expected">The enumerable to validate against</param>
    /// <param name="expectedExpr">Provided by the compiler for building the specification description</param>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> EqualTo(
        IEnumerable<TItem> expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Xunit.Assert.Equal(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that both enumerables has equal elements in any order
    /// </summary>
    /// <param name="expected">The enumerable to validate against</param>
    /// <param name="expectedExpr">Provided by the compiler for building the specification description</param>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> Like(
        IEnumerable<TItem> expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Xunit.Assert.Equivalent(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that both enumerables has equal elements in any order
    /// </summary>
    /// <param name="expected">The enumerable to validate against</param>
    /// <param name="expectedExpr">Provided by the compiler for building the specification description</param>
    /// <returns>A continuation for making further assertions on the enumerable</returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> EquivalentTo(
        IEnumerable<TItem> expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Xunit.Assert.Equivalent(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that both enumerables are the same instance
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsEnumerableContinuation<TItem>> SameAs(
        IEnumerable<TItem> expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Xunit.Assert.Same(expected, actual), expectedExpr!, methodName: string.Empty).And();

    internal override IsEnumerableContinuation<TItem> Continue() => Create(Actual, ActualExpr);
}