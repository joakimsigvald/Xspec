using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Xspec.Internal.TestData.Generation.Strategies;

internal readonly record struct CompiledProperty(
    Type PropertyType,
    Func<object, object?> Get,
    Action<object, object?> Set
);

internal static class PropertyAccessorCache
{
    private static readonly ConcurrentDictionary<Type, CompiledProperty[]> _cache = [];

    internal static CompiledProperty[] GetAccessors(Type type)
        => _cache.GetOrAdd(type, CompileProperties);

    private static CompiledProperty[] CompileProperties(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite
                     && p.GetSetMethod(nonPublic: false) != null
                     && p.GetIndexParameters().Length == 0);

        var compiledProperties = new List<CompiledProperty>();

        foreach (var prop in properties)
        {
            // 1. Compile the Getter: (object instance) => (object)((T)instance).Property
            var getInstanceParam = Expression.Parameter(typeof(object), "instance");
            var typedInstanceForGet = Expression.Convert(getInstanceParam, type);
            var propertyAccess = Expression.Property(typedInstanceForGet, prop);
            var castResultToObject = Expression.Convert(propertyAccess, typeof(object));

            var getter = Expression.Lambda<Func<object, object?>>(castResultToObject, getInstanceParam).Compile();

            // 2. Compile the Setter:
            var setInstanceParam = Expression.Parameter(typeof(object), "instance");
            var setValueParam = Expression.Parameter(typeof(object), "value");

            // THE FIX: Use Unbox to get a mutable reference to boxed structs!
            Expression typedInstanceForSet = type.IsValueType
                ? Expression.Unbox(setInstanceParam, type)
                : Expression.Convert(setInstanceParam, type);

            var typedValue = Expression.Convert(setValueParam, prop.PropertyType);

            var propertyAssign = Expression.Assign(Expression.Property(typedInstanceForSet, prop), typedValue);

            var setter = Expression.Lambda<Action<object, object?>>(propertyAssign, setInstanceParam, setValueParam).Compile();

            compiledProperties.Add(new CompiledProperty(prop.PropertyType, getter, setter));
        }

        return [.. compiledProperties];
    }
}