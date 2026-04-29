namespace Xspec.Internal.TestData;

internal class DataProvider : IDataProvider
{
    private readonly Dictionary<Type, Stack<Arrangement>> _generalDefaults = [];
    private readonly Dictionary<Type, Stack<Arrangement>> _inputDefaults = [];
    private readonly Dictionary<Type, Stack<Arrangement>> _subjectDefaults = [];

    public Type[] UsedTypes => [.. _subjectDefaults.Keys.Concat(_generalDefaults.Keys).Distinct()];

    internal void UseValue<TValue>(TValue value, For scope)
    {
        var type = typeof(TValue);
        var defaults = GetDefaults(scope);
        defaults[type] = new([new ValueArrangement(value)]);
        foreach (var iface in type.GetInterfaces())
            defaults[iface] = new([new ValueArrangement(value)]);
    }

    internal void UseFactory<TValue>(Func<TValue> factory, For scope)
    {
        var type = typeof(TValue);
        var defaults = GetDefaults(scope);
        if (defaults.TryGetValue(type, out var arr) && arr.Count > 0)
        {
            if (arr.Peek() is FactoryArrangement farr)
                defaults[type] = new([new FactoryArrangement(() =>
                {
                    farr.Factory();
                    return factory();
                })]);
            else
                defaults[type].Push(new FactoryArrangement(() => factory()));
        }
        else defaults[type] = new([new FactoryArrangement(() => factory())]);
    }

    private Dictionary<Type, Stack<Arrangement>> GetDefaults(For scope)
        => scope switch
        {
            For.Input => _inputDefaults,
            For.Subject => _subjectDefaults,
            For.All => _generalDefaults,
            _ => throw new NotImplementedException($"{scope}")
        };

    public bool TryGetValue(Type type, For scope, out object? val)
        => TryGetValueOfType(type, scope, out val) || TryGetValueOfTask(type, scope, out val);

    private bool TryGetValueOfType(Type type, For scope, out object? val)
        => scope switch
        {
            For.Input => TryGetValue(_inputDefaults, type, out val) || TryGetValue(_generalDefaults, type, out val),
            For.Subject => TryGetValue(_subjectDefaults, type, out val) || TryGetValue(_generalDefaults, type, out val),
            _ => throw new NotImplementedException($"{scope}")
        };

    private bool TryGetValueOfTask(Type type, For scope, out object? val)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = type.GetGenericArguments()[0];
            if (TryGetValue(innerType, scope, out var innerVal))
            {
                var fromResult = typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(innerType);
                val = fromResult.Invoke(null, [innerVal])!;
                return true;
            }
        }
        val = null;
        return false;
    }

    private static bool TryGetValue(Dictionary<Type, Stack<Arrangement>> arrangements, Type type, out object? val)
    {
        if (arrangements.TryGetValue(type, out var arr) && arr.Count > 0)
        {
            if (arr.Peek() is FactoryArrangement farr)
            {
                arrangements[type].Pop();
                arrangements[type] = new([new ValueArrangement(farr.Factory())]);
            }
            val = (arrangements[type].Peek() as ValueArrangement)?.Value;
            return true;
        }
        val = null;
        return false;
    }
}