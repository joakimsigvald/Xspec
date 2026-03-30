using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.Time;

/// <summary>
/// Object that allows an assertions to be made on the provided TimeSpan
/// </summary>
public record IsTimeSpan : IsComparable<TimeSpan, IsTimeSpan>
{
    /// <summary>
    /// Asserts that the timeSpan is within the specified precision time from the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="tolerance"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsTimeSpan> CloseTo(
        TimeSpan expected, TimeSpan tolerance, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(expected, actual => Xunit.Assert.True((Actual - expected).Duration() <= tolerance), expectedExpr!).And();

    /// <summary>
    /// Asserts that the timeSpan is less than zero
    /// </summary>
    /// <returns></returns>
    public ContinueWith<IsTimeSpan> Negative()
        => Assert(Ignore.Me, actual => Xunit.Assert.True(actual < TimeSpan.Zero)).And();

    /// <summary>
    /// Asserts that the timeSpan is greater than zero
    /// </summary>
    /// <returns></returns>
    public ContinueWith<IsTimeSpan> Positive()
        => Assert(Ignore.Me, actual => Xunit.Assert.True(actual > TimeSpan.Zero)).And();
}