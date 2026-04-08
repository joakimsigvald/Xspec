using Moq;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Mocking;

namespace Xspec.Internal.TestData;

internal class Repository
{
    private readonly Dictionary<Type, object?> _defaultValues = [];
    private readonly Dictionary<Type, Dictionary<int, object?>> _numberedMentions = [];
    private readonly AutoMockerWrapper _mocker;
    private readonly Dictionary<Type, Func<object, object>> _defaultSetups = [];
    private readonly DataGenerator _generator;

    public Repository()
    {
        _mocker = new(this);
        _generator = new(this, new(), new());
    }

    internal (object? val, bool found) Retrieve(Type type, int index = 0)
        => _numberedMentions.TryGetValue(type, out var map)
            && map.TryGetValue(index, out var val)
        ? (val, found: true)
        : (null, found: false);

    internal bool TryGetDefault(Type type, out object? val)
    {
        var found = _defaultValues.TryGetValue(type, out val);
        if (found)
            return true;

        if (!_defaultSetups.ContainsKey(type))
            return false;

        val = ApplyDefaultSetup(type, _generator.CreateNew(type));
        return true;
    }

    internal void Assign(Type type, object? value, int index)
    {
        SpecificationGenerator.Assign(type, index, value);
        GetMentions(type)[index] = value;
    }

    internal object? Instantiate<TValue>()
    {
        var type = typeof(TValue);
        var instance = DoInstantiate<TValue>();
        return ApplyDefaultSetup(type, instance);
    }

    private object? DoInstantiate<TValue>()
    {
        try
        {
            return _mocker.Instantiate<TValue>();
        }
        catch (Exception ex)
        {
            TestContext.Current?.AddWarning($"[Xspec] AutoMocking bypassed for {typeof(TValue).Name}. Fallback to DataGenerator. Reason: {ex.Message}");
            return null;
        }
    }

    internal void Use<TValue>(TValue value, For scope)
    {
        if (scope.HasFlag(For.Input))
            _defaultValues[typeof(TValue)] = value;

        if (value is Moq.Internals.InterfaceProxy)
            return;

        if (scope.HasFlag(For.Subject))
        {
            if (value is not null)
                _mocker.Use(value);
            else if (scope == For.Subject)
                throw new SetupFailed("Cannot use null");
        }

        if (!typeof(Task).IsAssignableFrom(typeof(TValue)))
            Use(Task.FromResult(value), scope);
    }

    internal void Use<TValue>(Func<TValue> factory, For scope)
    {
        Use<Func<TValue>>(factory, scope); //register factory as value
        Use(factory(), scope); //register factory-output as value
    }

    internal (object? val, bool found) Use(Type type)
        => TryGetDefault(type, out var value) ? (value, true) : (null, false);

    internal void AddDefaultSetup(Type type, Func<object, object> setup)
        => _defaultSetups[type] =
        _defaultSetups.TryGetValue(type, out var previousSetup)
        ? MergeDefaultSetups(previousSetup, setup)
        : setup;

    internal TValue Create<TValue>()
        => (TValue)ApplyDefaultSetup(typeof(TValue), MockOrCreate<TValue>())!;

    internal object Create(Type type) => ApplyDefaultSetup(type, _generator.Create(type))!;

    internal Mock<TObject> GetMock<TObject>() where TObject : class
        => _mocker.GetMock<TObject>();

    internal Mock GetMock(Type type) => _mocker.GetMock(type);

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _mocker.SetDefaultException(type, ex);

    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null)
        => _generator.Register(convert);

    private static Func<object, object> MergeDefaultSetups(Func<object, object> setup1, Func<object, object> setup2)
        => obj => setup2(setup1(obj));

    private Dictionary<int, object?> GetMentions(Type type)
        => _numberedMentions.TryGetValue(type, out var val) ? val : _numberedMentions[type] = [];

    private TValue MockOrCreate<TValue>()
        => typeof(TValue).IsInterface ? _mocker.Get<TValue>() : _generator.Create<TValue>();

    private object? ApplyDefaultSetup(Type type, object? newValue)
        => newValue is not null && _defaultSetups.TryGetValue(type, out var setup)
            ? setup(newValue)
            : newValue;
}