using Moq;
using System.Reflection;

namespace Xspec.Internal.TestData;

internal class FluentDefaultProvider : DefaultValueProvider
{
    private readonly DataProvider _dataProvider;

    internal FluentDefaultProvider(DataProvider dataProvider) => _dataProvider = dataProvider;

    protected override object GetDefaultValue(Type type, Mock mock)
    {
        var ex = _dataProvider.GetDefaultException(GetMockedType(mock));
        if (ex is not null)
            throw ex;
        var (val, found) = _dataProvider.Use(type);
        return found ? val!
            : IsReturningSelf(type, mock) ? mock.Object
            : IsTask(type) ? GetTask(type, mock)
            : _dataProvider.Create(type);
    }

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