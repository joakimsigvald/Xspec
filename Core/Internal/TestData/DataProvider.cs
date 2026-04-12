using Moq;
using Moq.AutoMock;
using Moq.AutoMock.Resolvers;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Generation.Strategies;
using Xspec.Internal.TestData.Mocking;

namespace Xspec.Internal.TestData;

internal class DataProvider
{
    private readonly DataGenerator _generator;
    private readonly Mutator _mutator;
    private readonly FluentDefaultProvider _fluentDefaultProvider;
    private readonly Dictionary<Type, Arrangement> _defaults = [];
    public DataProvider(Counter counter, Mutator mutator, TypeConversionStrategy typeConversionStrategy)
    {
        _generator = new(this, counter, typeConversionStrategy);
        _mutator = mutator;
        _fluentDefaultProvider = new(this);
    }

    internal AutoMocker Mocker { get; set; }

    internal object Create(Type type) => _mutator.Mutate(type, _generator.Create(type))!;

    internal TValue Create<TValue>()
        => (TValue)_mutator.Mutate(typeof(TValue), _generator.Create<TValue>())!;

    internal (object? val, bool found) Use(Type type)
        => TryGetDefault(type, out var value) ? (value, true) : (null, false);

    internal bool TryGetDefault(Type type, out object? val)
    {
        var found = TryGetValue(type, out val);
        if (found)
            return true;

        if (!_mutator.HasMutation(type))
            return false;

        val = _mutator.Mutate(type, _generator.CreateNew(type));
        return true;
    }

    internal void UseValue<TValue>(TValue value)
    {
        var type = typeof(TValue);
        if (_defaults.ContainsKey(type))
            return;

        _defaults[type] = new ValueArrangement(value);
    }

    internal void UseForMock<TValue>(Type type, TValue value)
    {
        Mocker.Use(value); //TODO: remove?
        if (type != value?.GetType()) //Explicit cast was provided, so don't use implicit cast to all interfaces
            return;

        var allInterfaces = type.GetInterfaces();
        foreach (var anInterface in allInterfaces)
            Mocker.Use(anInterface, value);
    }

    internal void UseFactory<TValue>(Func<TValue> factory)
    {
        var type = typeof(TValue);
        if (_defaults.TryGetValue(type, out var arr))
        {
            if (arr is FactoryArrangement farr)
            {
                _defaults[type] = new FactoryArrangement(() =>
                {
                    factory();
                    return farr.Factory();
                });
            }
            //Ignore factory if a value was already provided
        }
        else
            _defaults[type] = new FactoryArrangement(() => factory());
    }

    internal object? Instantiate<TValue>()
    {
        var type = typeof(TValue);
        var instance = DoInstantiate<TValue>();
        return _mutator.Mutate(type, instance);
    }

    internal Mock<TObject> GetMock<TObject>() where TObject : class => Mocker.GetMock<TObject>();
    internal Mock GetMock(Type type) => Mocker.GetMock(type);

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _fluentDefaultProvider.SetDefaultException(type, ex);

    private object? DoInstantiate<TValue>()
    {
        try
        {
            if (TryGetValue(typeof(TValue), out var val))
                return val;

            var type = typeof(TValue);
            if (type.IsValueType || type == typeof(string) || type.Namespace?.StartsWith("System") == true)
                return null;

            //if (type.IsInterface || type.IsAbstract)
            //    return GetMock(type).Object;

            return Mocker.CreateInstance(typeof(TValue));
        }
        catch (Exception ex)
        {
            TestContext.Current?.AddWarning($"[Xspec] AutoMocking bypassed for {typeof(TValue).Name}. Fallback to DataGenerator. Reason: {ex.Message}");
            return null;
        }
    }

    internal bool TryGetValue(Type type, out object? val)
    {
        if (!_defaults.TryGetValue(type, out var arr))
        {
            val = null;
            return false;
        }

        if (arr is FactoryArrangement farr) 
        {
            _defaults.Remove(type); //Make sure circular evaluation of factory value break on second pass
            _defaults[type] = arr = new ValueArrangement(farr.Factory());
        }

        val = (arr as ValueArrangement)?.Value;
        return true;
    }

    internal AutoMocker CreateAutoMocker()
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