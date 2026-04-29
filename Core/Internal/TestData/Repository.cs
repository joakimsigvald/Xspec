using Moq;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData.Generation.Strategies;

namespace Xspec.Internal.TestData;

internal class Repository
{
    private readonly Mutator _mutator = new();
    private readonly TypeConversionStrategy _typeConversionStrategy = new();
    private readonly DataProvider _dataProvider;
    private readonly MockProvider _mockProvider;
    private readonly Dictionary<Type, Dictionary<int, object?>> _numberedMentions = [];

    public Repository()
    {
        _dataProvider = new(_mutator);
        _mockProvider = new(_dataProvider);
        _dataProvider.Generator = new(new(), _typeConversionStrategy, new(_dataProvider, _mockProvider));
    }

    internal (object? val, bool found) Retrieve(Type type, int index = 0)
        => _numberedMentions.TryGetValue(type, out var map) && map.TryGetValue(index, out var val)
            ? (val, found: true)
            : (null, found: false);

    internal bool TryGetDefault(Type type, out object? val) => _dataProvider.TryGetDefault(type, For.Input, out val);

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

    internal void Use<TValue>(TValue value, For scope) => _dataProvider.UseValue(value, scope);

    internal void Use<TValue>(Func<TValue> factory, For scope)
    {
        Use<Func<TValue>>(factory, scope);
        _dataProvider.UseFactory(factory, scope);
    }

    internal void AddDefaultSetup(Type type, Func<object, object> mutation) => _mutator.AddMutation(type, mutation);

    internal TValue Create<TValue>() => _dataProvider.Create<TValue>(For.Input);

    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null)
        => _typeConversionStrategy.Register(convert);

    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mockProvider.GetMock<TObject>();

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _mockProvider.SetDefaultException(type, ex);

    private Dictionary<int, object?> GetMentions(Type type)
        => _numberedMentions.TryGetValue(type, out var val) ? val : _numberedMentions[type] = [];
}