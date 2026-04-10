using Moq;
using Moq.AutoMock;
using Moq.AutoMock.Resolvers;
using System.Collections.Concurrent;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Generation.Strategies;
using Xspec.Internal.TestData.Mocking;

namespace Xspec.Internal.TestData;

internal class DataProvider
{
    private readonly DataGenerator _generator;
    private readonly Mutator _mutator;
    private readonly AutoMocker _mocker;
    private readonly FluentDefaultProvider _fluentDefaultProvider;
    private readonly ConcurrentDictionary<Type, object?> _defaultValues = [];

    public DataProvider(Counter counter, Mutator mutator, TypeConversionStrategy typeConversionStrategy)
    {
        _generator = new(this, counter, typeConversionStrategy);
        _mutator = mutator;
        _fluentDefaultProvider = new(this);
        _mocker = CreateAutoMocker();
    }

    internal object Create(Type type) => _mutator.Mutate(type, _generator.Create(type))!;

    internal TValue Create<TValue>()
        => (TValue)_mutator.Mutate(typeof(TValue), _generator.Create<TValue>())!;

    internal (object? val, bool found) Use(Type type)
        => TryGetDefault(type, out var value) ? (value, true) : (null, false);

    internal bool TryGetDefault(Type type, out object? val)
    {
        var found = _defaultValues.TryGetValue(type, out val);
        if (found)
            return true;

        if (!_mutator.HasMutation(type))
            return false;

        val = _mutator.Mutate(type, _generator.CreateNew(type));
        return true;
    }

    internal void Use<TValue>(TValue value)
    {
        var type = typeof(TValue);
        if (_defaultValues.ContainsKey(type))
            return;

        _defaultValues[type] = value;
        if (value is null)
            return;

        _mocker.Use(value);
        if (type != value?.GetType()) //Explicit cast was provided, so don't use implicit cast to all interfaces
            return;

        var allInterfaces = type.GetInterfaces();
        foreach (var anInterface in allInterfaces)
            _mocker.Use(anInterface, value);
    }

    internal object? Instantiate<TValue>()
    {
        var type = typeof(TValue);
        var instance = DoInstantiate<TValue>();
        return _mutator.Mutate(type, instance);
    }

    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mocker.GetMock<TObject>();
    internal Mock GetMock(Type type) => _mocker.GetMock(type);

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _fluentDefaultProvider.SetDefaultException(type, ex);

    private object? DoInstantiate<TValue>()
    {
        try
        {
            if (_defaultValues.TryGetValue(typeof(TValue), out var val))
                return val;

            var type = typeof(TValue);
            if (type.IsValueType || type == typeof(string) || type.Namespace?.StartsWith("System") == true)
                return null;

            return _mocker.CreateInstance(typeof(TValue));
        }
        catch (Exception ex)
        {
            TestContext.Current?.AddWarning($"[Xspec] AutoMocking bypassed for {typeof(TValue).Name}. Fallback to DataGenerator. Reason: {ex.Message}");
            return null;
        }
    }

    private AutoMocker CreateAutoMocker()
    {
        var autoMocker = new AutoMocker(
            MockBehavior.Loose,
            DefaultValue.Custom,
            _fluentDefaultProvider,
            false);
        CustomizeResolvers(autoMocker);
        return autoMocker;
    }

    private void CustomizeResolvers(AutoMocker autoMocker)
    {
        var resolverList = (List<IMockResolver>)autoMocker.Resolvers;
        AddValueResolver();
        ReplaceArrayResolver();

        void AddValueResolver() =>
            resolverList.Insert(resolverList.Count - 1, new ValueResolver(this));

        void ReplaceArrayResolver()
            => resolverList[GetArrayResolverIndex()] = new EmptyArrayResolver();

        int GetArrayResolverIndex()
            => resolverList.FindIndex(_ => _.GetType() == typeof(ArrayResolver));
    }
}