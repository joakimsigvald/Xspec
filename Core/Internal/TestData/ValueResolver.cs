using Moq.AutoMock.Resolvers;

namespace Xspec.Internal.TestData;

internal class ValueResolver : IMockResolver
{
    private readonly DataProvider _context;

    internal ValueResolver(DataProvider context) => _context = context;

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