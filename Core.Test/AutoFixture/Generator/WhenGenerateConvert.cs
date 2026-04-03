using Xspec.Assert;
using Xspec.Internal.TestData.Generation;
using Xspec.Semantic;
using Xspec.Test.AutoFixture.Primitives;

namespace Xspec.Test.AutoFixture.Generator;

public abstract class WhenGenerateDerivedValueType<TValueType>(int maxDistinct) : Spec<TValueType>
{
    [Fact]
    public void GivenMaxDistinctValues_ThenAreDifferent()
        => Enumerable.Range(1, maxDistinct).Select(n => Any<TValueType>()).Is().Distinct();
}

public class WhenConvertByConstructor : WhenGenerateDerivedValueType<MyEmailConstr>
{
    public WhenConvertByConstructor() : base(100) => Register<MyEmailConstr>().As<Email>();
    [Fact] public void ThenHasValidEmailFormat() => An<MyEmailConstr>().Value.Has(ValidEmail.Verify);
}

public class WhenConvertByStaticCreate : WhenGenerateDerivedValueType<MyEmailConstr>
{
    public WhenConvertByStaticCreate() : base(100) => Register<MyEmailStatic>().As<Email>();
    [Fact] public void ThenHasValidEmailFormat() => An<MyEmailStatic>().Value.Has(ValidEmail.Verify);
}

public class WhenConvertByExplicitCast : WhenGenerateDerivedValueType<MyEmailConstr>
{
    public WhenConvertByExplicitCast() : base(100) => Register<MyEmailCast>().As<Email>();
    [Fact] public void ThenHasValidEmailFormat() => An<MyEmailCast>().Value.Has(ValidEmail.Verify);
}

public class WhenRelayIntToByte : Spec<int>
{
    public WhenRelayIntToByte() => Register<int>().As<byte>();
    [Fact] public void ThenGenerateByteAsInt() => Three<int>().Is().EqualTo([1, 2, 3]);
}

public class WhenRelayByteToInt : Spec<byte>
{
    public WhenRelayByteToInt() => Register<byte>().As<int>();
    [Fact] public void ThenGenerateIntAsByte() => Three<byte>().Is().EqualTo([1, 2, 3]);
}

public class WhenRelayStringToByte : Spec<string>
{
    public WhenRelayStringToByte() => Register<string>().As<byte>();
    [Fact] public void ThenGenerateByteAsString() => Three<string>().Is().EqualTo(["1", "2", "3"]);
}

public class WhenRelayIncompatibleTypes : Spec<byte>
{
    public WhenRelayIncompatibleTypes() => Register<byte>().As<DateTime>();

    [Fact]
    public void ThenThrowsInvalidTypeConversion()
        => Xunit.Assert.Throws<InvalidTypeConversion>(() => A<byte>().Is(0));
}

public record MyEmailConstr(string Value);
public record MyEmailStatic
{
    public string Value { get; private set; }
    public static MyEmailStatic Make(string value)
        => new() { Value = value };
}

public record MyEmailCast
{
    public string Value { get; private set; }
    public static implicit operator MyEmailCast(string value) => new() { Value = value };
}