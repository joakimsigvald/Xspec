using Moq;
using Moq.AutoMock;
using Moq.AutoMock.Resolvers;

namespace Xspec.Internal.TestData.Generation.Strategies.Mocking;

internal class MockingStrategy(ValueResolver valueResolver, FluentDefaultProvider fluentDefaultProvider) : IGenerationStrategy
{
    private readonly Lazy<AutoMocker> _mocker = new(() => CreateAutoMocker(valueResolver, fluentDefaultProvider));

    internal Mock GetMock(Type type) => _mocker.Value.GetMock(type);
    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mocker.Value.GetMock<TObject>();

    public bool TryGenerate(GenerationRequest request, ref object? result)
    {
        if (request.WithDefaultFallback && IsMockingResponsibility(request)) 
        {
            result = _mocker.Value.Get(request.Type);
            return true;
        }
        return false;
    }

    private static bool IsMockingResponsibility(GenerationRequest request)
        => request.Type.IsInterface
        || request.Type.IsAbstract
        || typeof(Delegate).IsAssignableFrom(request.Type)
        || request.Type.Name.StartsWith("Lazy`");

    private static AutoMocker CreateAutoMocker(ValueResolver valueResolver, FluentDefaultProvider fluentDefaultProvider)
    {
        var autoMocker = new AutoMocker(
            MockBehavior.Loose,
            DefaultValue.Custom,
            fluentDefaultProvider,
            false);
        CustomizeResolvers(autoMocker, valueResolver);
        return autoMocker;
    }

    private static void CustomizeResolvers(AutoMocker autoMocker, ValueResolver valueResolver)
    {
        var resolverList = (List<IMockResolver>)autoMocker.Resolvers;
        AddValueResolver();
        ReplaceArrayResolver();

        void AddValueResolver() =>
            resolverList.Insert(resolverList.Count - 1, valueResolver);

        void ReplaceArrayResolver()
            => resolverList[GetArrayResolverIndex()] = new EmptyArrayResolver();

        int GetArrayResolverIndex()
            => resolverList.FindIndex(_ => _.GetType() == typeof(ArrayResolver));
    }
}