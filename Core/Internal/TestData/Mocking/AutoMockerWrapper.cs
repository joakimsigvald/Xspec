using Moq;
using Moq.AutoMock;
using Moq.AutoMock.Resolvers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Xspec.Internal.TestData.Mocking;

internal class AutoMockerWrapper
{
    private readonly AutoMocker _mocker;
    private readonly ConcurrentBag<Type> _usages = [];

    internal AutoMockerWrapper(DataProvider context) => _mocker = CreateAutoMocker(context);

    internal void Use<TService>([DisallowNull] TService service)
    {
        var type = typeof(TService);
        if (_usages.Contains(type))
            return;

        _usages.Add(type);
        _mocker.Use(service);
        if (type != service.GetType()) //Explicit cast was provided, so don't use implicit cast to all interfaces
            return;

        var allInterfaces = type.GetInterfaces();
        foreach (var anInterface in allInterfaces)
            _mocker.Use(anInterface, service);
    }

    internal TValue Instantiate<TValue>() => (TValue)_mocker.CreateInstance(typeof(TValue));

    internal TValue Get<TValue>() => _mocker.Get<TValue>();
    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mocker.GetMock<TObject>();
    internal Mock GetMock(Type type) => _mocker.GetMock(type);

    private static AutoMocker CreateAutoMocker(DataProvider context)
    {
        var autoMocker = new AutoMocker(
            MockBehavior.Loose,
            DefaultValue.Custom,
            new FluentDefaultProvider(context),
            false);
        CustomizeResolvers(autoMocker, context);
        return autoMocker;
    }

    private static void CustomizeResolvers(AutoMocker autoMocker, DataProvider context)
    {
        var resolverList = (List<IMockResolver>)autoMocker.Resolvers;
        AddValueResolver();
        ReplaceArrayResolver();

        void AddValueResolver() =>
            resolverList.Insert(resolverList.Count - 1, new ValueResolver(context));

        void ReplaceArrayResolver()
            => resolverList[GetArrayResolverIndex()] = new EmptyArrayResolver();

        int GetArrayResolverIndex()
            => resolverList.FindIndex(_ => _.GetType() == typeof(ArrayResolver));
    }
}