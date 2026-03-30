using System.Runtime.CompilerServices;
using Xspec.Assert.Continuations;
using Xspec.Assert.Continuations.String;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on string
/// </summary>
public static class AssertionExtensionsString
{
    /// <summary>
    /// Verify that the string is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the string</returns>
    public static ContinueWith<IsStringContinuation> Is(
        this string actual,
        string expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Get available assertions for the given string
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsString Is(
        this string? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsString.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the characteristics of the given string
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static DoesString Does(
        this string? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => DoesString.Create(actual, actualExpr!);
}