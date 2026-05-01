namespace Xspec.Internal.TestData.Generation.Strategies;

internal class DefaultStrategy(Repository repository) : IGenerationStrategy
{
    public bool TryGenerate(GenerationRequest request, ref object? result)
        => request.WithDefaultFallback && repository.TryGetDefault(request.Type, request.Scope, out result);
}