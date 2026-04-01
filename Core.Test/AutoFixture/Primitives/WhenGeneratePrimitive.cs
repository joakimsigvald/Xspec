using Xspec.Assert;

namespace Xspec.Test.AutoFixture.Primitives;

public abstract class WhenGeneratePrimitive<TPrimitive>(int maxDistinct) : Spec<TPrimitive>
{
    [Fact] public void GivenSUT_ThenIsNotDefault() => When(_ => _).Then().Result.Is().Not(default(TPrimitive));
    [Fact] public void GivenSingle_ThenIsNotDefault() => A<TPrimitive>().Is().Not(default(TPrimitive));
    [Fact]
    public void GivenMaxDistinctValues_ThenAreDifferent()
        => Enumerable.Range(1, maxDistinct).Select(n => Any<TPrimitive>()).Is().Distinct();
}

public class WhenGenerateInt() : WhenGeneratePrimitive<int>(100);
public class WhenGenerateUInt() : WhenGeneratePrimitive<uint>(100);
public class WhenGenerateByte() : WhenGeneratePrimitive<byte>(100);
public class WhenGenerateSByte() : WhenGeneratePrimitive<sbyte>(100);
public class WhenGenerateShort() : WhenGeneratePrimitive<short>(100);
public class WhenGenerateUShort() : WhenGeneratePrimitive<ushort>(100);
public class WhenGenerateLong() : WhenGeneratePrimitive<long>(100);
public class WhenGenerateULong() : WhenGeneratePrimitive<ulong>(100);

public class WhenGenerateFloat() : WhenGeneratePrimitive<float>(100)
{
    [Fact] public void GivenSingle_ThenHasFraction() => A<float>().Has(_ => _ % 1 != 0);
}

public class WhenGenerateDouble() : WhenGeneratePrimitive<double>(100)
{
    [Fact] public void GivenSingle_ThenHasFraction() => A<double>().Has(_ => _ % 1 != 0);
}

public class WhenGenerateDecimal() : WhenGeneratePrimitive<decimal>(100)
{
    [Fact] public void GivenSingle_ThenHasFraction() => A<decimal>().Has(_ => _ % 1 != 0);
}

public class WhenGenerateBool() : WhenGeneratePrimitive<bool>(2);
public class WhenGenerateChar() : WhenGeneratePrimitive<char>(94)
{
    [Fact]
    public void GivenSingle_ThenIsPrintable()
        => A<char>().Is().GreaterThan((char)32).and.LessThan((char)127);
}

public class WhenGenerateDateTime() : WhenGeneratePrimitive<DateTime>(100)
{
    [Fact]
    public void GivenSingle_ThenIsContemporary()
            => A<DateTime>().Is().After(new DateTime(1900, 1, 1));
}

public class WhenGenerateTimeSpan() : WhenGeneratePrimitive<TimeSpan>(100);
public class WhenGenerateDateOnly() : WhenGeneratePrimitive<DateOnly>(100)
{
    [Fact]
    public void GivenSingle_ThenIsContemporary()
            => A<DateOnly>().Is().After(new DateOnly(1900, 1, 1));
}

public class WhenGenerateTimeOnly() : WhenGeneratePrimitive<TimeOnly>(100);
public class WhenGenerateGuid() : WhenGeneratePrimitive<Guid>(100);
public class WhenGenerateString() : WhenGeneratePrimitive<string>(100)
{
    [Fact] public void GivenSingle_ThenIsNotEmpty() => A<string>().Is().not.Empty();
}