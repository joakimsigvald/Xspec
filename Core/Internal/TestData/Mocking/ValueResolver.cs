using Moq.AutoMock.Resolvers;

namespace Xspec.Internal.TestData.Mocking;

internal class ValueResolver(IDataProvider context) : IMockResolver
{
    public void Resolve(MockResolutionContext context)
    {
        if (context.RequestType.IsValueType || context.RequestType == typeof(string))
            context.Value = GetValue(context.RequestType);
    }

    private object GetValue(Type type)
    {
        var (val, found) = context.Use(type, For.Subject);
        return found ? val! : context.Create(type, For.Subject);
    }
}