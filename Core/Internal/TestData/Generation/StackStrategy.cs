namespace Xspec.Internal.TestData.Generation;

internal class StackStrategy : IGenerationStrategy
{
    public bool TryGenerate(GenerationRequest request, ref object? result)
        => request.Stack.Contains(request.Type);
}
