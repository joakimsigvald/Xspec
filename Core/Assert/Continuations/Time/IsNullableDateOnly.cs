using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.Time;

/// <summary>
/// Object that allows an assertions to be made on the provided nullable DateOnly
/// </summary>
public record IsNullableDateOnly : IsNullableComparableStruct<DateOnly, IsNullableDateOnly, IsDateOnly>
{
    /// <summary>
    /// Asserts that the nullable DateOnly is before the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateOnly> Before(
        DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x < 0, expectedExpr!, "occur");

    /// <summary>
    /// Asserts that the nullable DateOnly is after the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateOnly> After(
        DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x > 0, expectedExpr!, "occur");
}