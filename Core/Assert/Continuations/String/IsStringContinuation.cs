using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.String;

/// <summary>
/// Object that allows another assertions to be made on the provided string
/// </summary>
public record IsStringContinuation : IsString
{
    /// <summary>
    /// Continuation to assert that the string does satisfy some expectation
    /// </summary>
    /// <returns></returns>
    public DoesString Does() => Actual.Does(actualExpr: ActualExpr);

    /// <summary>
    /// Verify that the string is same as expected
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the string</returns>
    public ContinueWith<IsStringContinuation> Is(
        string expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Actual.Is(actualExpr: ActualExpr).Value(expected, expectedExpr!);
}