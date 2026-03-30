namespace Xspec.Assert.Continuations.String;

/// <summary>
/// Object that allows another assertions to be made on the provided string
/// </summary>
public record DoesStringContinuation : DoesString
{
    /// <summary>
    /// Continuation to assert that the string is satisfying some expectation
    /// </summary>
    /// <returns></returns>
    public IsString Is() => Actual.Is(actualExpr: ActualExpr);
}