using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations.Numerical.Fractional;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TActual"></typeparam>
/// <typeparam name="TIsFractional"></typeparam>
public abstract record IsFractional<TActual, TIsFractional> : IsNumerical<TActual, TIsFractional>
    where TActual : struct, IComparable<TActual>
    where TIsFractional : IsFractional<TActual, TIsFractional>, new()
{
    /// <summary>
    /// Asserts that the actual value is approximately equal to the given value, within the provided precision
    /// </summary>
    /// <param name="expected">Expected value</param>
    /// <param name="tolerance">Allowed difference +/- expected value</param>
    /// <param name="expectedExpr">Ignore, provided by the compiler</param>
    /// <returns></returns>
    public ContinueWith<TIsFractional> Around(
        TActual expected, TActual tolerance, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(expected, actual => AssertEqual(expected, actual, tolerance), expectedExpr!).And();

    private protected abstract void AssertEqual(TActual expected, TActual actual, TActual tolerance);
    private protected abstract void AssertNotEqual(TActual expected, TActual actual, TActual tolerance);
}