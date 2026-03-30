using System.Runtime.CompilerServices;

namespace Xspec.Assert.Continuations;

/// <summary>
/// Object that allows an assertions to be made on the provided nullable Enum
/// </summary>
public record IsNullableStruct<TValue> : Constraint<TValue?, IsNullableStruct<TValue>>
    where TValue : struct
{
    /// <summary>
    /// Assert that the value is not same as the given value
    /// </summary>
    public ContinueWith<IsNullableStruct<TValue>> Not(
        TValue expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Xunit.Assert.NotEqual(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that the value is not same as the given value
    /// </summary>
    public ContinueWith<IsNullableStruct<TValue>> Not(
        TValue? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Xunit.Assert.NotEqual(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that the value is null
    /// </summary>
    public ContinueWith<IsNullableStruct<TValue>> Null()
        => Assert(Ignore.Me, Xunit.Assert.Null).And();
}