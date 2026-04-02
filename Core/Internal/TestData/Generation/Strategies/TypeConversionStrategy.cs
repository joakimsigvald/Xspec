namespace Xspec.Internal.TestData.Generation.Strategies;

internal class TypeConversionStrategy() : IGenerationStrategy
{
    private readonly Dictionary<Type, Type> _typeRelays = [];

    public bool TryGenerate(GenerationRequest request, ref object? result)
    {
        if (!_typeRelays.TryGetValue(request.Type, out var sourceType))
            return false;

        result = request.Create(sourceType);
        if (request.Type.IsAssignableFrom(sourceType))
            return true;

        result = GetCandidateValues(result)
            .Select(TryConstruct)
            .FirstOrDefault(v => v != null)
            ?? TryCast(result)
            ?? throw new InvalidTypeConversion(request.Type, sourceType);

        return true;

        IEnumerable<object?> GetCandidateValues(object? sourceVal)
            => sourceType
                .GetMethods(
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.FlattenHierarchy)
                .Where(m => m.Name == "op_Implicit" && m.GetParameters().Length == 1)
                .Select(m => m.Invoke(null, [sourceVal]))
                .Prepend(sourceVal);

        object? TryConvert(object? argument) => TryConstruct(argument) ?? TryCast(argument);

        object? TryConstruct(object? argument)
            => request.Type.GetConstructor([argument?.GetType() ?? sourceType])?.Invoke([argument]);

        object? TryCast(object? argument)
        {
            try { return Convert.ChangeType(argument, request.Type); }
            catch { return null; }
        }
    }

    internal void Register<TTarget, TSource>() => _typeRelays[typeof(TTarget)] = typeof(TSource);
}