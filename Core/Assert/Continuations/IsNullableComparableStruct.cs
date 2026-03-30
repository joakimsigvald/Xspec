using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations;

/// <summary>
/// base class that allows an assertions to be made on the provided nullable numerical
/// </summary>
/// <typeparam name="TActual"></typeparam>
/// <typeparam name="TContinuation"></typeparam>
/// <typeparam name="TValueContinuation"></typeparam>
public abstract record IsNullableComparableStruct<TActual, TContinuation, TValueContinuation>
    : Constraint<TActual?, TContinuation>
    where TActual : struct, IComparable<TActual>
    where TContinuation : IsNullableComparableStruct<TActual, TContinuation, TValueContinuation>, new()
    where TValueContinuation : IsComparable<TActual, TValueContinuation>, new()
{
    private TValueContinuation ValueContinuation => Constraint<TActual, TValueContinuation>.Create(Actual!.Value, ActualExpr);

    /// <summary>
    /// Assert that the value is null
    /// </summary>
    public ContinueWith<TContinuation> Null() => Assert(Ignore.Me, Xunit.Assert.Null).And();

    /// <summary>
    /// Assert that the value is not equal to the given value
    /// </summary>
    public ContinueWith<TContinuation> Not(
        TActual? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(expected, actual => Xunit.Assert.NotEqual(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that the value is not equal to the given value
    /// </summary>
    public ContinueWith<TValueContinuation> Not(
        TActual expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x != 0, expectedExpr!);

    /// <summary>
    /// Assert that the value is greater than the given value
    /// </summary>
    public ContinueWith<TValueContinuation> GreaterThan(
        TActual expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x > 0, expectedExpr!);

    /// <summary>
    /// Assert that the value is less than the given value
    /// </summary>
    public ContinueWith<TValueContinuation> LessThan(
        TActual expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => CompareTo(expected, x => x < 0, expectedExpr!);

    internal ContinueWith<TValueContinuation> Value(
        TActual expected, string expectedExpr)
    {
        Assert(() =>
            {
                try
                {
                    Xunit.Assert.True(Actual.HasValue);
                    Xunit.Assert.Equal(expected, Actual.Value);
                }
                catch
                {
                    Xunit.Assert.Fail($"Expected {ActualExpr} to be {expected} but found {Actual}");
                }
            }, string.Empty, expectedExpr);
        return new(ValueContinuation);
    }

    private protected ContinueWith<TValueContinuation> CompareTo(
        TActual expected,
        Func<int, bool> comparer,
        string expectedExpr,
        string auxVerb = "be",
        [CallerMemberName] string? methodName = null)
    {
        Assert(expected, NotNullAnd(actual => Xunit.Assert.True(comparer(actual!.Value.CompareTo(expected)))), 
            expectedExpr, auxVerb, methodName: methodName);
        return new(ValueContinuation);
    }
}