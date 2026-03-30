namespace Xspec.Assert.Continuations.Time;

/// <summary>
/// Object that allows an assertions to be made on the provided nullable TimeSpan
/// </summary>
public record IsNullableTimeSpan : IsNullableComparableStruct<TimeSpan, IsNullableTimeSpan, IsTimeSpan>;