using Moq;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Generation.Strategies;

namespace Xspec.Internal.TestData;

internal class DataProvider
{
    private readonly DataGenerator _generator;
    private readonly Mutator _mutator;
    private readonly Dictionary<Type, Stack<Arrangement>> _defaults = [];

    public DataProvider(Counter counter, Mutator mutator, TypeConversionStrategy typeConversionStrategy)
    {
        _generator = new(this, counter, typeConversionStrategy);
        _mutator = mutator;
    }

    internal MockProvider MockProvider { get; set; }

    internal IEnumerable<Type> UsedTypes => _defaults.Keys;

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
        foreach (var iface in type.GetInterfaces())
            _defaults[iface] = new([new ValueArrangement(value)]);
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

    internal Mock GetMock(Type type) => MockProvider.GetMock(type);

    internal bool TryGetValue(Type type, out object? val)
    {
        if (_defaults.TryGetValue(type, out var arr) && arr.Count > 0)
        {
            if (arr.Peek() is FactoryArrangement farr)
            {
                _defaults[type].Pop();
                _defaults[type] = new([new ValueArrangement(farr.Factory())]);
            }

            val = (_defaults[type].Peek() as ValueArrangement)?.Value;
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = type.GetGenericArguments()[0];
            if (TryGetValue(innerType, out var innerVal))
            {
                var fromResult = typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(innerType);
                val = fromResult.Invoke(null, [innerVal])!;
                return true;
            }
        }

        val = null;
        return false;
    }
}