using System.Runtime.CompilerServices;
using Xspec.Assert.Continuations;
using Xspec.Assert.Continuations.Numerical.Nullable;
using CallerArgument = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on nullable integer type numbers
/// </summary>
public static class AssertionExtensionsNullableNumerical
{
    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableByte> Is(
        this byte? actual,
        byte? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableSByte> Is(
        this sbyte? actual,
        sbyte? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableUShort> Is(
        this ushort? actual,
        ushort? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableShort> Is(
        this short? actual,
        short? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableUInt> Is(
        this uint? actual,
        uint? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableInt> Is(
        this int? actual,
        int? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableULong> Is(
        this ulong? actual,
        ulong? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected and return continuation for further assertions of the value
    /// </summary>
    public static ContinueWith<IsNullableLong> Is(
        this long? actual,
        long? expected,
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
    public static IsNullableByte Is(
        this byte? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableByte.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableSByte Is(
        this sbyte? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableSByte.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableShort Is(
        this short? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableShort.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableUShort Is(
        this ushort? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableUShort.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableInt Is(
        this int? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableInt.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableUInt Is(
        this uint? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableUInt.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableLong Is(
        this long? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableLong.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableULong Is(
        this ulong? actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsNullableULong.Create(actual, actualExpr!);
}