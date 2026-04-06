using Moq;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData.Generation;
using Xspec.Internal.TestData.Mocking;

namespace Xspec.Internal.TestData;

internal class DataProvider
{
    private readonly Dictionary<Type, object?> _defaultValues = [];
    private readonly Dictionary<Type, Func<Exception>> _defaultExceptions = [];
    private readonly Dictionary<Type, Dictionary<int, object?>> _numberedMentions = [];
    private readonly AutoMockerWrapper _testDataGenerator;
    private readonly Dictionary<Type, Func<object, object>> _defaultSetups = [];
    private readonly DataGenerator _generator;

    public DataProvider()
    {
        _testDataGenerator = new(this);
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

    internal TValue Instantiate<TValue>()
    {
        var type = typeof(TValue);
        var instance = TryGetDefault(typeof(TValue), out var val)
            ? val
            : DoInstantiate<TValue>();
        return (TValue)(ApplyDefaultSetup(type, instance!) ?? default!);
    }

    internal void Use<TValue>(TValue value, Scope scope)
    {
        if (scope.HasFlag(Scope.Input))
            _defaultValues[typeof(TValue)] = value;

        if (value is Moq.Internals.InterfaceProxy)
            return;

        if (scope.HasFlag(Scope.Subject))
        {
            if (value is not null)
                _testDataGenerator.Use(value);
            else if (scope == Scope.Subject)
                throw new SetupFailed("Cannot use null");
        }

        if (typeof(Task).IsAssignableFrom(typeof(TValue)))
            return;

        Use(Task.FromResult(value), scope);
    }

    internal (object? val, bool found) Use(Type type)
        => TryGetDefault(type, out var value) ? (value, true) : (null, false);

    internal void AddDefaultSetup(Type type, Func<object, object> setup)
        => _defaultSetups[type] =
        _defaultSetups.TryGetValue(type, out var previousSetup)
        ? MergeDefaultSetups(previousSetup, setup)
        : setup;

    internal TValue Create<TValue>()
        => (TValue)ApplyDefaultSetup(typeof(TValue), MockOrCreate<TValue>()!);

    internal object Create(Type type) => ApplyDefaultSetup(type, _generator.Create(type));

    internal Mock<TObject> GetMock<TObject>() where TObject : class
        => _testDataGenerator.GetMock<TObject>();

    internal Mock GetMock(Type type) => _testDataGenerator.GetMock(type);

    internal Exception? GetDefaultException(Type type)
        => _defaultExceptions.TryGetValue(type, out var ex) ? ex() : null;

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _defaultExceptions[type] = ex;

    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null) 
        => _generator.Register(convert);

    private static Func<object, object> MergeDefaultSetups(Func<object, object> setup1, Func<object, object> setup2)
        => obj => setup2(setup1(obj));

    private Dictionary<int, object?> GetMentions(Type type)
        => _numberedMentions.TryGetValue(type, out var val) ? val : _numberedMentions[type] = [];

    private TValue DoInstantiate<TValue>()
    {
        var type = typeof(TValue);
        if (type.IsValueType || type == typeof(string) || type.Namespace?.StartsWith("System") == true)
            return _generator.Create<TValue>();

        try
        {
            return _testDataGenerator.Instantiate<TValue>();
        }
        catch (Exception ex)
        {
            TestContext.Current?.AddWarning($"[Xspec] AutoMocking bypassed for {type.Name}. Fallback to DataGenerator. Reason: {ex.Message}"); 
            return _generator.Create<TValue>();
        }
    }

    private TValue MockOrCreate<TValue>()
        => typeof(TValue).IsInterface ? _testDataGenerator.Get<TValue>() : _generator.Create<TValue>();

    private object ApplyDefaultSetup(Type type, object newValue)
        => _defaultSetups.TryGetValue(type, out var setup)
            ? setup(newValue)
            : newValue;
}