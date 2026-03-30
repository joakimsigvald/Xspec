namespace Xspec.Assert.Continuations.Enumerable;

/// <summary>
/// Object that allows another assertions to be made on the provided string
/// </summary>
public record HasEnumerableContinuation<TItem> : HasEnumerable<TItem>
{
    /// <summary>
    /// Get available assertions for the given enumerable
    /// </summary>
    /// <returns></returns>
    public IsEnumerable<TItem> Is() => Actual.Is(actualExpr: ActualExpr);
    /// <summary>
    /// Get available assertions for the given enumerable
    /// </summary>
    /// <returns></returns>
    public DoesEnumerable<TItem> Does() => Actual.Does(actualExpr: ActualExpr);
}