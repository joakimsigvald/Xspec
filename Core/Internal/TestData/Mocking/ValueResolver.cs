using Moq.AutoMock.Resolvers;

namespace Xspec.Internal.TestData.Mocking;

internal class ValueResolver : IMockResolver
{
    private readonly Repository _context;

    internal ValueResolver(Repository context) => _context = context;

    public void Resolve(MockResolutionContext context)
    {
        if (context.RequestType.IsValueType || context.RequestType == typeof(string))
            context.Value = GetValue(context.RequestType);
    }

    private object GetValue(Type type)
    {
        var (val, found) = _context.Use(type);
        return found ? val! : _context.Create(type);
    }
}