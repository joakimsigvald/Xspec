using Xspec.Internal.TestData.Generation.Strategies;

namespace Xspec.Internal.TestData.Generation;

internal class DataGenerator(DataProvider context, Counter counter, TypeConversionStrategy typeConversionStrategy)
{
    private static readonly StackStrategy _stackStrategy = new();
    private static readonly NullableStrategy _nullableStrategy = new();
    private static readonly EnumStrategy _enumStrategy = new();
    private static readonly CollectionStrategy _collectionStrategy = new();
    private static readonly ObjectStrategy _objectStrategy = new();

    private readonly IGenerationStrategy[] _strategies = [
        typeConversionStrategy,
        new DefaultStrategy(context),
        _stackStrategy,
        _nullableStrategy,
        _enumStrategy,
        new PrimitiveStrategy(counter),
        new SemanticTypeStrategy(counter),
        _collectionStrategy,
        _objectStrategy,
        ];

    internal TValue Create<TValue>() => (TValue)Create(typeof(TValue))!;
    internal object? Create(Type type) => Create(new GenerationRequest(type, true, [], this));
    internal object? CreateNew(Type type) => Create(new GenerationRequest(type, false, [], this));

    internal object? Create(GenerationRequest request)
    {
        object? val = null;
        foreach (var strategy in _strategies)
            if (strategy.TryGenerate(request, ref val))
                return val;
        return val;
    }
}