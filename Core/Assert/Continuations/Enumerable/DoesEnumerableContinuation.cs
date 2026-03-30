namespace Xspec.Assert.Continuations.Enumerable;

/// <summary>
/// Object that allows another assertions to be made on the provided string
/// </summary>
public record DoesEnumerableContinuation<TItem> : DoesEnumerable<TItem>
{
    /// <summary>
    /// Get available asserts fo the given enumerable
    /// </summary>
    /// <returns></returns>
    public HasEnumerable<TItem> Has() => Actual.Has(actualExpr: ActualExpr);
    /// <summary>
    /// Get available asserts fo the given enumerable
    /// </summary>
    /// <returns></returns>
    public IsEnumerable<TItem> Is() => Actual.Is(actualExpr: ActualExpr);
}