using Moq.AutoMock.Resolvers;

namespace Xspec.Internal.TestData.Mocking;

internal class ValueResolver(Repository repository) : IMockResolver
{
    public void Resolve(MockResolutionContext context)
    {
        if (context.RequestType.IsValueType || context.RequestType == typeof(string))
            context.Value = GetValue(context.RequestType);
    }

    private object GetValue(Type type)
    {
        var (val, found) = repository.Use(type, For.Subject);
        return found ? val! : repository.Create(type, For.Subject);
    }
}