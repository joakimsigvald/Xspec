using Xspec.Assert.Continuations.Numerical.Nullable;

namespace Xspec.Assert.Continuations.Numerical.Fractional.Nullable;

/// <summary>
/// Object that allows an assertions to be made on the provided nullable decimal
/// </summary>
public record IsNullableDecimal : IsNullableNumerical<decimal, IsNullableDecimal, IsDecimal>;