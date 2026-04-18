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
    private readonly Dictionary<Type, Stack<Arrangement>> _defaults = [];
    private bool _isMockerPreparedForInstantiation = false;

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
        _defaults[type] = new([new ValueArrangement(value)]);
    }

    internal void UseFactory<TValue>(Func<TValue> factory)
    {
        var type = typeof(TValue);
        if (_defaults.TryGetValue(type, out var arr) && arr.Count > 0)
        {
            if (arr.Peek() is FactoryArrangement farr)
                _defaults[type] = new([new FactoryArrangement(() =>
                {
                    farr.Factory();
                    return factory();
                })]);
            else
                _defaults[type].Push(new FactoryArrangement(() => factory()));
        }
        else _defaults[type] = new([new FactoryArrangement(() => factory())]);
    }

    internal Mock GetMock(Type type) => Mocker.GetMock(type);

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _fluentDefaultProvider.SetDefaultException(type, ex);

    internal object? Instantiate(Type type)
    {
        PrepareMockerForInstantiation();
        try
        {
            if (TryGetValue(type, out var val))
                return val;

            if (type.IsValueType || type == typeof(string) || type.Namespace?.StartsWith("System") == true)
                return null;

            return Mocker.CreateInstance(type);
        }
        catch (Exception ex)
        {
            TestContext.Current?.AddWarning($"[Xspec] AutoMocking bypassed for {type.Name}. Fallback to DataGenerator. Reason: {ex.Message}");
            return null;
        }
    }

    internal bool TryGetValue(Type type, out object? val)
    {
        if (_defaults.TryGetValue(type, out var arr) && arr.Count > 0)
        {
            if (arr.Peek() is FactoryArrangement farr)
            {
                _defaults[type].Pop();
                _defaults[type].Push(new ValueArrangement(farr.Factory()));
            }

            val = (_defaults[type].Peek() as ValueArrangement)?.Value;
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = type.GetGenericArguments()[0];
            if (TryGetValue(innerType, out var innerVal))
            {
                val = CreateCompletedTask(innerType, innerVal);
                return true;
            }
        }

        val = null;
        return false;
    }

    private static object CreateCompletedTask(Type innerType, object? value)
        => typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(innerType).Invoke(null, [value])!;

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

    private void PrepareMockerForInstantiation()
    {
        if (_isMockerPreparedForInstantiation)
            return;

        _isMockerPreparedForInstantiation = true;
        Dictionary<Type, object?> mockValues = [];
        Type[] types = [.. _defaults.Keys
            .SelectMany(t => new[] { t, typeof(Task<>).MakeGenericType(t) })
            .Distinct()];

        foreach (var type in types)
        {
            if (!TryGetValue(type, out var val))
                continue;

            var actualType = val?.GetType();
            mockValues[type] = val;
            if (type == actualType)
            {
                var allInterfaces = type.GetInterfaces();
                foreach (var anInterface in allInterfaces)
                    mockValues[anInterface] = val;
            }
        }
        foreach (var kvp in mockValues)
        {
            try
            {
                Mocker.Use(kvp.Key, kvp.Value);
            }
            catch (InvalidOperationException ioex) when (ioex.Message.Contains("The service instance has already been added"))
            {
                TestContext.Current?.AddWarning($"[Xspec] Mocker.Use ignored, value has already been added: {ioex.Message}");
            }
        }
    }
}