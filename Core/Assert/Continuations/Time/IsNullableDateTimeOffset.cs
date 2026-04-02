using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.Time;

/// <summary>
/// Object that allows an assertions to be made on the provided nullable DateTime
/// </summary>
public record IsNullableDateTimeOffset : IsNullableComparableStruct<DateTimeOffset, IsNullableDateTimeOffset, IsDateTimeOffset>
{
    /// <summary>
    /// Asserts that the nullable DateTimeOffset is before the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateTimeOffset> Before(
        DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x < 0, expectedExpr!, "occur");

    /// <summary>
    /// Asserts that the nullable DateTimeOffset is after the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateTimeOffset> After(
        DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x > 0, expectedExpr!, "occur");
}