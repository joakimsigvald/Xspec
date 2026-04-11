namespace Xspec.Internal.TestData;

internal record FactoryArrangement(Func<object?> Factory) : Arrangement { }
