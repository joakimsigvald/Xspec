using Moq.AutoMock.Resolvers;

namespace Xspec.Internal.TestData.Generation.Strategies.Mocking;

internal class EmptyArrayResolver : IMockResolver
{
    public void Resolve(MockResolutionContext context)
    {
        if (context.RequestType.IsArray && context.RequestType != typeof(string))
            context.Value = CreateEmptyArray(context.RequestType);
    }

    private static Array CreateEmptyArray(Type type)
    {
        Type elmType = type.GetElementType()
            ?? throw new InvalidOperationException($"Could not determine element type for '{type}'");
        return Array.CreateInstance(elmType, new int[type.GetArrayRank()]);
    }
}