namespace Xspec.Internal.TestData.Generation.Strategies;

internal class DefaultStrategy(Repository repository, MockProvider mockProvider) : IGenerationStrategy
{
    public bool TryGenerate(GenerationRequest request, ref object? result)
        => request.WithDefaultFallback && TryGetDefault(request.Type, request.Scope, out result);

    private bool TryGetDefault(Type type, For scope, out object? val)
    {
        if (repository.TryGetDefault(type, scope, out val))
            return true;

        if (type.IsInterface || type.IsAbstract)
        {
            var (instance, found) = repository.Use(type, scope);
            try
            {
                val = found ? instance! : mockProvider.GetMock(type).Object;
                return true;
            }
            catch { }
        }
        val = null;
        return false;
    }
}