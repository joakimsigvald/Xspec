using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.String;

/// <summary>
/// Object that allows further assertions to be made on the provided string
/// </summary>
public record IsStringContinuation : IsString
{
    /// <summary>
    /// Continuation to assert that the string does satisfy some expectation
    /// </summary>
    /// <returns>A continuation for further assertions of the value</returns>
    public DoesString Does() => Actual.Does(actualExpr: ActualExpr);

    /// <summary>
    /// Verify that the string is same as expected
    /// </summary>
    /// <param name="expected">The expected value</param>
    /// <param name="expectedExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>Continuation for further assertions of the string</returns>
    public ContinueWith<IsStringContinuation> Is(
        string? expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Actual.Is(actualExpr: ActualExpr).Value(expected!, expectedExpr!);
}