using Moq;
using System.Reflection;

namespace Xspec.Internal.TestData.Mocking;

internal class FluentDefaultProvider(Repository repository) : DefaultValueProvider
{
    private readonly Dictionary<Type, Func<Exception>> _defaultExceptions = [];

    protected override object GetDefaultValue(Type type, Mock mock)
    {
        var ex = GetDefaultException(GetMockedType(mock));
        if (ex is not null)
            throw ex;
        var (val, found) = repository.Use(type, For.Subject);
        return found ? val!
            : IsReturningSelf(type, mock) ? mock.Object
            : IsTask(type) ? GetTask(type, mock)
            : repository.Create(type, For.Subject);
    }

    private Exception? GetDefaultException(Type type)
        => _defaultExceptions.TryGetValue(type, out var ex) ? ex() : null;

    internal void SetDefaultException(Type type, Func<Exception> ex)
        => _defaultExceptions[type] = ex;

    private static bool IsReturningSelf(Type type, Mock mock)
        => !type.IsAssignableFrom(typeof(object)) && type.IsAssignableFrom(mock.Object.GetType());

    private static bool IsTask(Type type) => typeof(Task).IsAssignableFrom(type);

    private Task GetTask(Type type, Mock mock)
        => type == typeof(Task)
        ? Task.CompletedTask
        : GetTaskOf(type.GenericTypeArguments.Single(), mock);

    private Task GetTaskOf(Type valueType, Mock mock)
    {
        dynamic value = GetDefaultValue(valueType, mock);
        if (value.GetType() != valueType) 
        {
            var mockName = GetMockedType(mock).Name;
            throw new SetupFailed(
                @$"{mockName} returns a Task<{valueType.Name}>. 
Interface types returned as task must be provided explicitly in the test setup.
You can provide a default interface instance with 'Given<{mockName}>().Returns(A<{valueType.Name}>)'.");
        }
        return Task.FromResult(value);
    }

    private static Type GetMockedType(Mock mock)
    {
        var mockType = mock.GetType();
        var mockedTypeProperty = 
            mockType.GetProperty("MockedType", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Failed to get mocked type property of type {mockType}");
        return mockedTypeProperty.GetValue(mock) as Type 
            ?? throw new InvalidOperationException($"Failed to get type of mock property {mockedTypeProperty}");
    }
}