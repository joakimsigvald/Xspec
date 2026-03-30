using Moq;
using Moq.AutoMock;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Xspec.Internal.TestData;

internal class TestDataGenerator
{
    private readonly AutoMocker _mocker;
    private readonly DataGenerator _generator;
    private readonly DataGenerator _defaultGenerator;
    private readonly ConcurrentBag<Type> _usages = [];

    internal TestDataGenerator(DataGenerator fixture, AutoMocker mocker)
    {
        _mocker = mocker;
        _generator = fixture;
        _defaultGenerator = CreateDefaultFixture();
    }

    private static DataGenerator CreateDefaultFixture() => new();

    internal TValue Create<TValue>()
        => typeof(TValue).IsInterface ? _mocker.Get<TValue>() : CreateValue<TValue>();

    internal void Use<TService>([DisallowNull] TService service)
    {
        var type = typeof(TService);
        if (_usages.Contains(type))
            return;

        _usages.Add(type);
        _mocker.Use(service);
        if (type != service.GetType()) //Explicit cast was provided, so don't use implicit cast to all interfaces
            return;

        var allInterfaces = type.GetInterfaces();
        foreach (var anInterface in allInterfaces)
            _mocker.Use(anInterface, service);
    }

    internal TValue Instantiate<TValue>()
    {
        try
        {
            return (TValue)_mocker.CreateInstance(typeof(TValue));
        }
        catch (ArgumentException ex) when (ex.Message.Contains("Did not find a best constructor for"))
        {
            return (TValue)Create(typeof(TValue));
        }
    }

    internal object Create(Type type)
    {
        try
        {
            return _generator.Create(type);
        }
        catch (Exception ex)
        {
            return CreateDefaultValue(type, ex);
        }
    }

    internal object CreateDefault(Type type)
    {
        try
        {
            return _defaultGenerator.Create(type);
        }
        catch (Exception ex)
        {
            return CreateDefaultValue(type, ex);
        }
    }

    private TValue CreateValue<TValue>()
    {
        try
        {
            return _generator.Create<TValue>();
        }
        catch (Exception ex)
        {
            return (TValue)CreateDefaultValue(typeof(TValue), ex);
        }
    }

    private static object CreateDefaultValue(Type type, Exception ex)
    {
        try
        {
            return Activator.CreateInstance(type)!;
        }
        catch (ArgumentException)
        {
            throw new SetupFailed($"Failed to create value for type {type.Name}", ex);
        }
    }

    internal Mock<TObject> GetMock<TObject>() where TObject : class => _mocker.GetMock<TObject>();
    internal Mock GetMock(Type type) => _mocker.GetMock(type);
}