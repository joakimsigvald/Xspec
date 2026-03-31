namespace Xspec.Internal.TestData.Generation;

internal class DefaultStrategy(DataProvider context) : IGenerationStrategy
{
    public bool TryGenerate(GenerationRequest request, ref object? result)
        => request.WithDefaultFallback && TryGetDefault(request.Type, out result);

    internal bool TryGetDefault(Type type, out object? val)
    {
        if (context.TryGetDefault(type, out val))
            return true;

        if (type.IsInterface)
        {
            var (instance, found) = context.Use(type);
            try
            {
                val = found ? instance! : context.GetMock(type).Object;
                return true;
            }
            catch { }
        }
        val = null;
        return false;
    }
}
