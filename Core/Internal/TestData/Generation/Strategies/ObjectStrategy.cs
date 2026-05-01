using System.Collections.Concurrent;
using System.Reflection;

namespace Xspec.Internal.TestData.Generation.Strategies;

internal class ObjectStrategy : IGenerationStrategy
{
    private static readonly ConcurrentDictionary<Type, ConstructorInfo?> _greediestConstructors = [];
    
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
            => _greediestConstructors.GetOrAdd(type, FindGreediestConstructor);

        ConstructorInfo? FindGreediestConstructor(Type type)
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
                catch (Exception)
                {
                    throw new SetupFailed($"Failed to create value for type {type.Name}", ex);
                }
            }
        }

        void PopulatePublicProperties(object instance)
        {
            var accessors = PropertyAccessorCache.GetAccessors(type);
            foreach (var accessor in accessors)
            {
                var currentValue = accessor.Get(instance);
                var emptyValue = _defaultCache.GetOrAdd(accessor.PropertyType, _defaultFactory);
                if (Equals(emptyValue, currentValue))
                {
                    var newValue = request.Next.Create(accessor.PropertyType);
                    accessor.Set(instance, newValue);
                }
            }
        }
    }
}