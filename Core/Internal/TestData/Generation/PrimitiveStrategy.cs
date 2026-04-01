namespace Xspec.Internal.TestData.Generation;

internal class PrimitiveStrategy : IGenerationStrategy
{
    private int _counter = 0;
    private static readonly DateTime _epoch = new(1999, 9, 18, 12, 0, 0, DateTimeKind.Utc);
    private const int _hundredYears = 36523;
    private const int _oneDay = 60 * 24;

    public bool TryGenerate(GenerationRequest request, ref object? result)
    {
        result = GeneratePrimitive(request.Type);
        return result is not null || TryGenerateBCLStructs(ref result);


        bool TryGenerateBCLStructs(ref object? result)
        {
            if (request.Type == typeof(TimeSpan)) { result = TimeSpan.FromDays(++_counter); return true; }
            if (request.Type == typeof(DateOnly)) { result = DateOnly.FromDateTime(GetDateTime()); return true; }
            if (request.Type == typeof(TimeOnly)) { result = TimeOnly.FromDateTime(GetDateTime()); return true; }
            return false;
        }
    }

    private object? GeneratePrimitive(Type type)
        => Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => ++_counter % 2 == 1,
            TypeCode.DateTime => GetDateTime(),
            TypeCode.Int32 => ++_counter,
            TypeCode.UInt32 => (uint)++_counter,
            TypeCode.Byte => (byte)++_counter,
            TypeCode.SByte => (sbyte)++_counter,
            TypeCode.Int16 => (short)++_counter,
            TypeCode.UInt16 => (ushort)++_counter,
            TypeCode.Int64 => (long)++_counter,
            TypeCode.UInt64 => (ulong)++_counter,
            TypeCode.Single => (float)GetFractional(),
            TypeCode.Double => (double)GetFractional(),
            TypeCode.Decimal => GetFractional(),
            _ => null
        };

    private decimal GetFractional() => ++_counter + (1 + Math.Abs(++_counter) % 99) / 100m;

    private DateTime GetDateTime() => _epoch.AddDays((++_counter * 367) % _hundredYears).AddMinutes(_counter * 17 % _oneDay);
}