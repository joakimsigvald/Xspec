using Xspec.Assert.Continuations;
using Xspec.Assert.Continuations.Numerical;
using CallerArgument = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on integer type numbers
/// </summary>
public static class AssertionExtensionsNumerical
{
    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsInt> Is(
        this int actual,
        int expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsByte> Is(
        this byte actual,
        byte expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsSByte> Is(
        this sbyte actual,
        sbyte expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsShort> Is(
        this short actual,
        short expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsUShort> Is(
        this ushort actual,
        ushort expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsLong> Is(
        this long actual,
        long expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that the value is same as expected
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr">Ignore, provided by runtime</param>
    /// <param name="expectedExpr">Ignore, provided by runtime</param>
    /// <returns>A continuation for further assertions of the value</returns>
    public static ContinueWith<IsULong> Is(
        this ulong actual,
        ulong expected,
        [CallerArgument(nameof(actual))] string? actualExpr = null,
        [CallerArgument(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsByte Is(
        this byte actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsByte.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsSByte Is(
        this sbyte actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsSByte.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsShort Is(
        this short actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsShort.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsUShort Is(
        this ushort actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsUShort.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsInt Is(
        this int actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsInt.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsUInt Is(
        this uint actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsUInt.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsLong Is(
        this long actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsLong.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsULong Is(
        this ulong actual,
        Ignore _ = default,
        [CallerArgument(nameof(actual))] string? actualExpr = null)
        => IsULong.Create(actual, actualExpr!);
}