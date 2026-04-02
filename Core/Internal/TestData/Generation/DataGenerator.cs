using Xspec.Internal.TestData.Generation.Strategies;

namespace Xspec.Internal.TestData.Generation;

internal class DataGenerator(DataProvider context, Counter counter, TypeConversionStrategy typeConversionStrategy)
{
    private readonly IGenerationStrategy[] _strategies = [
        typeConversionStrategy,
        new DefaultStrategy(context),
        new StackStrategy(),
        new NullableStrategy(),
        new EnumStrategy(),
        new PrimitiveStrategy(counter),
        new SemanticTypeStrategy(counter),
        new CollectionStrategy(),
        new ObjectStrategy(),
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

    internal void Register<TTarget, TSource>() => typeConversionStrategy.Register<TTarget, TSource>();
}