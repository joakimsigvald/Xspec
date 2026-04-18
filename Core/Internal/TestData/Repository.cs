using Moq;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Generation.Strategies;

namespace Xspec.Internal.TestData;

internal class Repository
{
    private readonly Mutator _mutator = new();
    private readonly TypeConversionStrategy _typeConversionStrategy = new();
    private readonly DataProvider _inputProvider;
    private readonly DataProvider _subjectProvider;
    private readonly Dictionary<Type, Dictionary<int, object?>> _numberedMentions = [];

    public Repository()
    {
        Counter counter = new();
        _subjectProvider = new(counter, _mutator, _typeConversionStrategy);
        var mocker = _subjectProvider.CreateAutoMocker();
        _subjectProvider.Mocker = mocker;
        _inputProvider = new(counter, _mutator, _typeConversionStrategy) { Mocker = mocker };
    }

    internal (object? val, bool found) Retrieve(Type type, int index = 0)
    {
        return _numberedMentions.TryGetValue(type, out var map) && map.TryGetValue(index, out var val)
            ? (val, found: true)
            : (null, found: false);
    }

    internal bool TryGetDefault(Type type, out object? val) => _inputProvider.TryGetDefault(type, out val);

    internal void Assign(Type type, object? value, int index)
    {
        SpecificationGenerator.Assign(type, index, value);
        GetMentions(type)[index] = value;
    }

    internal object? Instantiate<TValue>() => _subjectProvider.Instantiate<TValue>();

    internal void Use<TValue>(TValue value, For scope)
    {
        if (scope.HasFlag(For.Input))
            _inputProvider.UseValue(value);

        if (scope.HasFlag(For.Subject))
            _subjectProvider.UseValue(value);
    }

    internal void Use<TValue>(Func<TValue> factory, For scope)
    {
        Use<Func<TValue>>(factory, scope); //register factory as value

        if (scope.HasFlag(For.Input))
            _inputProvider.UseFactory(factory);

        if (scope.HasFlag(For.Subject))
            _subjectProvider.UseFactory(factory);
    }

    internal void AddDefaultSetup(Type type, Func<object, object> mutation) => _mutator.AddMutation(type, mutation);

    internal TValue Create<TValue>() => _inputProvider.Create<TValue>();

    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null)
        => _typeConversionStrategy.Register(convert);

    internal Mock<TObject> GetMock<TObject>() where TObject : class => _subjectProvider.GetMock<TObject>();

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _subjectProvider.SetDefaultException(type, ex);

    private Dictionary<int, object?> GetMentions(Type type)
        => _numberedMentions.TryGetValue(type, out var val) ? val : _numberedMentions[type] = [];
}