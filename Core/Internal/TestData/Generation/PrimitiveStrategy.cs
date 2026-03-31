namespace Xspec.Internal.TestData.Generation;

internal class PrimitiveStrategy : IGenerationStrategy
{
    private int _counter = 0;

    public bool TryGenerate(GenerationRequest request, ref object? result)
    {
        var type = request.Type;
        if (type == typeof(string)) { result = $"String_{++_counter}"; return true; }
        if (type == typeof(int)) { result = ++_counter; return true; }
        if (type == typeof(bool)) { result = ++_counter % 2 == 0; return true; }
        if (type == typeof(DateTime)) { result = DateTime.UtcNow.AddDays(++_counter); return true; }
        if (type == typeof(TimeSpan)) { result = TimeSpan.FromDays(++_counter); return true; }
        return false;
    }
}
