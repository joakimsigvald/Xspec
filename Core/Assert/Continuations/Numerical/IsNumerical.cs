namespace Xspec.Assert.Continuations.Numerical;

/// <summary>
/// Base class that allows an assertions to be made on the provided numerical
/// </summary>
/// <typeparam name="TContinuation"></typeparam>
/// <typeparam name="TActual"></typeparam>
public abstract record IsNumerical<TActual, TContinuation> : IsComparable<TActual, TContinuation>
    where TContinuation : IsNumerical<TActual, TContinuation>, new()
    where TActual : struct, IComparable<TActual>
{
    /// <summary>
    /// Assert that the value is greater than expected
    /// </summary>
    public ContinueWith<TContinuation> Even()
        => Assert(null, NotNullAnd(actual => Xunit.Assert.Equal(0, Convert.ToInt64(actual) % 2))).And();
}