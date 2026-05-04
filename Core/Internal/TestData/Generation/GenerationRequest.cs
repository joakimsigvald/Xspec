using System.Collections.Immutable;

namespace Xspec.Internal.TestData.Generation;

internal record GenerationRequest(
    Type Type,
    bool WithDefaultFallback,
    ImmutableStack<Type> Stack,
    DataGenerator Orchestrator,
    For Scope,
    int Depth = 0)
{
    internal object? Create(Type type) => Orchestrator.Create(this with { Type = type });
    internal GenerationRequest Next => this with { WithDefaultFallback = true, Stack = Stack.Push(Type), Depth = this.Depth + 1 };
}