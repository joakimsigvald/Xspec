namespace Xspec.Assert.Continuations.Numerical.Nullable;

/// <summary>
/// base class that allows assertions to be made on the provided nullable numerical
/// </summary>
/// <typeparam name="TActual">The type of the value to assert on</typeparam>
/// <typeparam name="TContinuation">The concrete type of the assertion continuation, enabling fluent chaining</typeparam>
/// <typeparam name="TValueContinuation">The concrete type of the assertion continuation, enabling fluent chaining</typeparam>
public abstract record IsNullableNumerical<TActual, TContinuation, TValueContinuation>
    : IsNullableComparableStruct<TActual, TContinuation, TValueContinuation>
    where TActual : struct, IComparable<TActual>
    where TContinuation : IsNullableNumerical<TActual, TContinuation, TValueContinuation>, new()
    where TValueContinuation : IsNumerical<TActual, TValueContinuation>, new();