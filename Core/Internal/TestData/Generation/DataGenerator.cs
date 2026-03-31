using System.Reflection;

namespace Xspec.Internal.TestData.Generation;

internal class DataGenerator(DataProvider _context)
{
    private int _counter = 0;

    internal TValue Create<TValue>() => (TValue)Create(typeof(TValue))!;

    internal object? Create(Type type, params Type[] stack)
        => TryGetDefault(type, out var defaultVal) ? defaultVal! : CreateNew(type, stack);

    internal object? CreateNew(Type type, params Type[] stack)
    {
        if (stack.Any(t => t == type))
            return null!;

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
            return Create(underlyingType, stack);

        if (type.IsEnum) return GenerateEnumValue(type);
        if (type == typeof(string)) return GenerateString();
        if (type == typeof(int)) return GenerateInt();
        if (type == typeof(bool)) return GenerateBool();
        if (type == typeof(DateTime)) return GenerateDateTime();
        if (type == typeof(TimeSpan)) return GenerateTimeSpan();
        if (type.IsArray) return GenerateArray(type.GetElementType()!);
        if (IsGenericEnumerable(type)) return GenerateArray(type.GetGenericArguments()[0]);
        return InstantiateType(type, stack);
    }

    private bool TryGetDefault(Type type, out object? val)
    {
        if (_context.TryGetDefault(type, out val))
            return true;

        if (type.IsInterface)
        {
            var (instance, found) = _context.Use(type);
            try
            {
                val = found ? instance! : _context.GetMock(type).Object;
                return true;
            }
            catch { }
        }
        val = null;
        return false;
    }

    private static object GenerateEnumValue(Type type)
    {
        var values = Enum.GetValues(type);
        return values.Length > 0 ? values.GetValue(0)! : Activator.CreateInstance(type)!;
    }

    private static bool IsGenericEnumerable(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDef = type.GetGenericTypeDefinition();
        return genericTypeDef == typeof(IEnumerable<>) ||
            genericTypeDef == typeof(IList<>) ||
            genericTypeDef == typeof(ICollection<>) ||
            genericTypeDef == typeof(IReadOnlyCollection<>) ||
            genericTypeDef == typeof(IReadOnlyList<>);
    }

    private static Array GenerateArray(Type elementType)
        => Array.CreateInstance(elementType, 0);

    private object? InstantiateType(Type type, params Type[] stack)
    {
        var instance = InstantiateWithConstructor(type, stack)
            ?? InstantiateWithConversionOperator(type, stack)
            ?? (type.IsValueType ? Activator.CreateInstance(type) : null);
        if (instance != null)
            PopulatePublicProperties(type, instance, stack);

        return instance;
    }

    private object? InstantiateWithConstructor(Type type, params Type[] stack)
    {
        var constructor = GetGreediestConstructor(type);
        return constructor is null ? null! : DoInstantiate(type, constructor, stack);
    }

    private object? InstantiateWithConversionOperator(Type type, params Type[] stack)
    {
        var castOperator = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.ReturnType == type);

        if (castOperator != null)
        {
            var parameter = castOperator.GetParameters().First();
            var paramValue = Create(parameter.ParameterType, [type, .. stack]);
            if (paramValue != null)
                return castOperator.Invoke(null, [paramValue]);
        }
        return null;
    }

    private object? DoInstantiate(Type type, ConstructorInfo constructor, params Type[] stack)
    {
        var parameters = constructor.GetParameters()
            .Select(p => Create(p.ParameterType, [type, .. stack]))
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

    private void PopulatePublicProperties(Type type, object instance, params Type[] stack)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite
            && p.GetSetMethod(nonPublic: false) != null
            && p.GetIndexParameters().Length == 0);

        foreach (var prop in properties)
        {
            var currentValue = prop.GetValue(instance);
            var propertyType = prop.PropertyType;
            var emptyValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
            if (!Equals(currentValue, emptyValue))
                continue;

            prop.SetValue(instance, Create(propertyType, stack));
        }
    }

    private static ConstructorInfo? GetGreediestConstructor(Type type)
        => type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

    private string GenerateString() => $"String_{++_counter}";
    private int GenerateInt() => ++_counter;
    private bool GenerateBool() => ++_counter % 2 == 0;
    private DateTime GenerateDateTime() => DateTime.UtcNow.AddDays(++_counter);
    private TimeSpan GenerateTimeSpan() => TimeSpan.FromDays(++_counter);
}