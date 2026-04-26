using Moq;
using Moq.AutoMock;
using Moq.AutoMock.Resolvers;
using Xspec.Internal.TestData.Mocking;

namespace Xspec.Internal.TestData;

internal class MockProvider
{
    private readonly DataProvider _dataProvider;
    private readonly FluentDefaultProvider _fluentDefaultProvider;
    private bool _isMockerPreparedForInstantiation = false;
    private readonly Lazy<AutoMocker> _mocker;

    public MockProvider(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
        _fluentDefaultProvider = new(dataProvider);
        _mocker = new(CreateAutoMocker);
    }

    internal Mock GetMock(Type type) => _mocker.Value.GetMock(type);
    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mocker.Value.GetMock<TObject>();

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _fluentDefaultProvider.SetDefaultException(type, ex);

    internal object? Instantiate(Type type)
    {
        PrepareMockerForInstantiation();
        try
        {
            if (_dataProvider.TryGetValue(type, out var val))
                return val;

            if (type.IsValueType || type == typeof(string) || type.Namespace?.StartsWith("System") == true)
                return null;

            return _mocker.Value.CreateInstance(type);
        }
        catch (Exception ex)
        {
            TestContext.Current?.AddWarning($"[Xspec] AutoMocking bypassed for {type.Name}. Fallback to DataGenerator. Reason: {ex.Message}");
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
            resolverList.Insert(resolverList.Count - 1, new ValueResolver(_dataProvider));

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
        Type[] types = [.. _dataProvider.UsedTypes
            .SelectMany(t => new[] { t, typeof(Task<>).MakeGenericType(t) })
            .Distinct()];

        foreach (var type in types)
        {
            if (!_dataProvider.TryGetValue(type, out var val))
                continue;

            var actualType = val?.GetType();
            mockValues[type] = val;
        }
        foreach (var kvp in mockValues)
        {
            try
            {
                _mocker.Value.Use(kvp.Key, kvp.Value);
            }
            catch (InvalidOperationException ioex) when (ioex.Message.Contains("The service instance has already been added"))
            {
                TestContext.Current?.AddWarning($"[Xspec] Mocker.Use ignored, value has already been added: {ioex.Message}");
            }
        }
    }
}