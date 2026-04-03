namespace Xspec.Internal.TestData.Generation.Strategies;

internal class TypeConversionStrategy() : IGenerationStrategy
{
    private readonly Dictionary<Type, TypeRelay> _typeRelays = [];

    public bool TryGenerate(GenerationRequest request, ref object? result)
    {
        if (!_typeRelays.TryGetValue(request.Type, out var source))
            return false;

        result = request.Create(source.Type);

        if (source.Convert != null)
        {
            result = source.Convert(result);
            return true;
        }

        if (request.Type.IsAssignableFrom(source.Type))
            return true;

        result = GetCandidateValues(result)
            .Select(TryConvert)
            .FirstOrDefault(v => v != null)
            ?? TryChangeType(result)
            ?? throw new InvalidTypeConversion(request.Type, source.Type);

        return true;

        IEnumerable<object?> GetCandidateValues(object? sourceVal)
            => source.Type
                .GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.FlattenHierarchy)
                .Where(m => m.Name == "op_Implicit" && m.GetParameters().Length == 1)
                .Select(m => m.Invoke(null, [sourceVal]))
                .Prepend(sourceVal);

        object? TryConvert(object? argument)
            => TryConstruct(argument)
            ?? TryStatic(argument);

        object? TryStatic(object? argument)
        {
            var argType = argument?.GetType() ?? source.Type;
            var method = request.Type
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .FirstOrDefault(m => m.ReturnType == request.Type
                && m.GetParameters() is [{ ParameterType: var pt }]
                && pt.IsAssignableFrom(argType));
            try { return method?.Invoke(null, [argument]); }
            catch { return null; }
        }

        object? TryConstruct(object? argument)
            => request.Type.GetConstructor([argument?.GetType() ?? source.Type])?.Invoke([argument]);

        object? TryChangeType(object? argument)
        {
            try { return Convert.ChangeType(argument, request.Type); }
            catch { return null; }
        }
    }

    internal void Register<TTarget, TSource>(Func<TSource, TTarget>? convert = null)
        => _typeRelays[typeof(TTarget)] = new(typeof(TSource), convert is null ? null : s => convert((TSource)s));
}

internal record TypeRelay(Type Type, Func<object?, object?>? Convert = null);