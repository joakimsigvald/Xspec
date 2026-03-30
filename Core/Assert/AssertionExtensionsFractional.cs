using System.Runtime.CompilerServices;
using Xspec.Assert.Continuations;
using Xspec.Assert.Continuations.Numerical.Fractional;
using Xspec.Assert.Continuations.Numerical.Fractional.Nullable;
using CallerArgument = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on decimal type numbers
/// </summary>
public static class AssertionExtensionsFractional
{
    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsFloat Is(
        this float actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsFloat.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsDouble Is(
        this double actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsDouble.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsDecimal Is(
        this decimal actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsDecimal.Create(actual, actualExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableFloat> Is(
        this float? actual,
        float? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableDouble> Is(
        this double? actual,
        double? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableDecimal> Is(
        this decimal? actual,
        decimal? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableFloat Is(
        this float? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableFloat.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableDouble Is(
        this double? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableDouble.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableDecimal Is(
        this decimal? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableDecimal.Create(actual, actualExpr!);
}