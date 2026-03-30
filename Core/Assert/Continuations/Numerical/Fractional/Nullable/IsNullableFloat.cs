using Xspec.Assert.Continuations.Numerical.Nullable;

namespace Xspec.Assert.Continuations.Numerical.Fractional.Nullable;

/// <summary>
/// Object that allows an assertions to be made on the provided nullable float
/// </summary>
public record IsNullableFloat : IsNullableNumerical<float, IsNullableFloat, IsFloat>;