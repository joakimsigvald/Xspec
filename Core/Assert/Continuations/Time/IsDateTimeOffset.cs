using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.Time;

/// <summary>
/// Object that allows an assertions to be made on the provided DateOnly
/// </summary>
public record IsDateTimeOffset : IsComparable<DateTimeOffset, IsDateTimeOffset>
{
    /// <summary>
    /// Asserts that the DateTimeOffset is before the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateTimeOffset> Before(
        DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x < 0, expectedExpr!, "occur");

    /// <summary>
    /// Asserts that the DateTimeOffset is after the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateTimeOffset> After(
        DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x > 0, expectedExpr!, "occur");

    /// <summary>
    /// Asserts that the DateTimeOffset is within the specified precision time from the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="tolerance"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsDateTimeOffset> CloseTo(
        DateTimeOffset expected, TimeSpan tolerance, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(expected, actual => Xunit.Assert.Equal(expected, actual, tolerance), expectedExpr!).And();
}