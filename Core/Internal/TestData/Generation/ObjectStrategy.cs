using System.Collections.Concurrent;
using System.Reflection;

namespace Xspec.Internal.TestData.Generation;

internal class ObjectStrategy : IGenerationStrategy
{
    private static readonly ConcurrentDictionary<Type, object?> _defaultCache = [];
    private static readonly Func<Type, object?> _defaultFactory =
        t => t.IsValueType ? Activator.CreateInstance(t) : null;

    public bool TryGenerate(GenerationRequest request, ref object? result)
    {
        var type = request.Type;
        var stack = request.Stack;
        result = InstantiateWithConstructor()
            ?? InstantiateWithConversionOperator()
            ?? (type.IsValueType ? Activator.CreateInstance(request.Type) : null);
        if (result != null)
            PopulatePublicProperties(result);

        return result is not null;

        object? InstantiateWithConstructor()
        {
            var constructor = GetGreediestConstructor();
            return constructor is null ? null! : DoInstantiate(constructor);
        }

        ConstructorInfo? GetGreediestConstructor()
            => type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

        object? InstantiateWithConversionOperator()
        {
            var castOperator = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.ReturnType == type);

            if (castOperator != null)
            {
                var parameter = castOperator.GetParameters().First();
                var paramValue = request.Next.Create(parameter.ParameterType);
                if (paramValue != null)
                    return castOperator.Invoke(null, [paramValue]);
            }
            return null;
        }

        object? DoInstantiate(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters()
                .Select(p => request.Next.Create(p.ParameterType))
                .ToArray();
            try
            {
                return constructor.Invoke(parameters);
            }
            catch (Exception ex)
            {
                try
                {
                    return Activator.CreateInstance(type)!;
                }
                catch (ArgumentException)
                {
                    throw new SetupFailed($"Failed to create value for type {type.Name}", ex);
                }
            }
        }

        void PopulatePublicProperties(object instance)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite
                && p.GetSetMethod(nonPublic: false) != null
                && p.GetIndexParameters().Length == 0);

            foreach (var prop in properties)
            {
                var currentValue = prop.GetValue(instance);
                var emptyValue = _defaultCache.GetOrAdd(prop.PropertyType, _defaultFactory);
                if (Equals(emptyValue, currentValue))
                    prop.SetValue(instance, request.Next.Create(prop.PropertyType));
            }
        }
    }
}