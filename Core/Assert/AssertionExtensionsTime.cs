using System.Runtime.CompilerServices;
using Xspec.Assert.Continuations;
using Xspec.Assert.Continuations.Time;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on DateTime and TimeSpan
/// </summary>
public static class AssertionExtensionsTime
{
    /// <summary>
    /// Verify that actual is expected timeSpan and return continuation for further assertions of the value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the TimeSpan?</returns>
    public static ContinueWith<IsNullableTimeSpan> Is(
        this TimeSpan? actual, TimeSpan? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected timeSpan and return continuation for further assertions of the value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the TimeSpan?</returns>
    public static ContinueWith<IsTimeSpan> Is(
        this TimeSpan? actual, TimeSpan expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected dateTime and return continuation for further assertions of the value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the DateTime?</returns>
    public static ContinueWith<IsNullableDateTime> Is(
        this DateTime? actual, DateTime? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected dateTime and return continuation for further assertions of the value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the DateTime?</returns>
    public static ContinueWith<IsDateTime> Is(
        this DateTime? actual, DateTime expected,
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
    public static IsDateTime Is(
        this DateTime actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsDateTime.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableDateTime Is(
        this DateTime? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsNullableDateTime.Create(actual, actualExpr!);


    /// <summary>
    /// Verify that actual is expected DateOnly and return continuation for further assertions of the value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the DateOnly?</returns>
    public static ContinueWith<IsNullableDateOnly> Is(
        this DateOnly? actual, DateOnly? expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual is expected DateOnly and return continuation for further assertions of the value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the DateOnly?</returns>
    public static ContinueWith<IsDateOnly> Is(
        this DateOnly? actual, DateOnly expected,
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
    public static IsDateOnly Is(
        this DateOnly actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsDateOnly.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableDateOnly Is(
        this DateOnly? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsNullableDateOnly.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsNullableTimeSpan Is(
        this TimeSpan? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsNullableTimeSpan.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given value
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsTimeSpan Is(
        this TimeSpan actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsTimeSpan.Create(actual, actualExpr!);
}