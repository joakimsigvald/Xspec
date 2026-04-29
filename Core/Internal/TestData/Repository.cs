using Moq;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Generation.Strategies;
using Xspec.Internal.TestData.Mocking;

namespace Xspec.Internal.TestData;

internal class Repository
{
    private readonly Mutator _mutator = new();
    private readonly TypeConversionStrategy _typeConversionStrategy = new();
    private readonly DataProvider _dataProvider;
    private readonly MockProvider _mockProvider;
    private readonly DataGenerator _generator;
    private readonly FluentDefaultProvider _fluentDefaultProvider;
    private readonly Dictionary<Type, Dictionary<int, object?>> _numberedMentions = [];

    public Repository()
    {
        _dataProvider = new();
        _fluentDefaultProvider = new(this);
        _mockProvider = new(_dataProvider, new(this), _fluentDefaultProvider);
        _generator = new(new(), _typeConversionStrategy, new(this, _mockProvider));
    }

    internal (object? val, bool found) Retrieve(Type type, int index = 0)
        => _numberedMentions.TryGetValue(type, out var map) && map.TryGetValue(index, out var val)
            ? (val, found: true)
            : (null, found: false);

    internal void Assign(Type type, object? value, int index)
    {
        SpecificationGenerator.Assign(type, index, value);
        GetMentions(type)[index] = value;
    }

    internal object? Instantiate<TValue>()
    {
        var type = typeof(TValue);
        var instance = _mockProvider.Instantiate(type);
        return _mutator.Mutate(type, instance);
    }

    internal bool TryGetDefault(Type type, For scope, out object? val)
    {
        var found = _dataProvider.TryGetValue(type, scope, out val);
        if (found)
            return true;

        if (!_mutator.HasMutation(type))
            return false;

        val = _mutator.Mutate(type, _generator.CreateNew(type, scope));
        return true;
    }

    internal void Use<TValue>(TValue value, For scope) => _dataProvider.UseValue(value, scope);

    internal void Use<TValue>(Func<TValue> factory, For scope)
    {
        Use<Func<TValue>>(factory, scope);
        _dataProvider.UseFactory(factory, scope);
    }

    internal void AddDefaultSetup(Type type, Func<object, object> mutation) => _mutator.AddMutation(type, mutation);

    public (object? val, bool found) Use(Type type, For scope)
        => TryGetDefault(type, scope, out var value) ? (value, true) : (null, false);

    public object Create(Type type, For scope) => _mutator.Mutate(type, _generator.Create(type, scope))!;

    internal TValue Create<TValue>(For scope)
        => (TValue)_mutator.Mutate(typeof(TValue), _generator.Create<TValue>(scope))!;


    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null)
        => _typeConversionStrategy.Register(convert);

    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mockProvider.GetMock<TObject>();

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _fluentDefaultProvider.SetDefaultException(type, ex);

    private Dictionary<int, object?> GetMentions(Type type)
        => _numberedMentions.TryGetValue(type, out var val) ? val : _numberedMentions[type] = [];
}