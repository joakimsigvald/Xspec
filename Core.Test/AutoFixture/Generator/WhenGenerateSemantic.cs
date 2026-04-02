using System.Text.RegularExpressions;
using Xspec.Assert;
using Xspec.Internal.TestData.Generation;
using Xspec.Semantic;

namespace Xspec.Test.AutoFixture.Primitives;

public abstract class WhenGenerateSemantic<TSemantic, TPrimitive>(int maxDistinct, Action<TSemantic> verify) : Spec<TSemantic>
    where TSemantic : Semantic<TPrimitive>, ISemantic<TPrimitive>
{
    [Fact]
    public void GivenSUT_ThenIsNotDefault()
        => When(_ => _).Then().Result.Value.Is().Not(default(TSemantic));

    [Fact]
    public void GivenMaxDistinctValues_ThenAreDifferent()
        => Enumerable.Range(1, maxDistinct).Select(n => Any<TSemantic>()).Is().Distinct().and.Has().All(verify);

    [Fact]
    public void ThenImplicitlyCastsToPrimitive()
        => A<TSemantic>().Value.Is((TPrimitive)The<TSemantic>());
}

public partial class WhenGenerateEmail() : WhenGenerateSemantic<Email, string>(
    100, e => _emailRegex.IsMatch(e).Is(true))
{
    private static readonly Regex _emailRegex = EmailRegex();
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

public partial class WhenGeneratePhoneNumber() : WhenGenerateSemantic<PhoneNumber, string>(100, p => _phoneNumberE164Regex.IsMatch(p).Is(true))
{
    private static readonly Regex _phoneNumberE164Regex = PhoneNumberE164Regex();
    [GeneratedRegex(@"^\+[1-9]\d{1,14}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PhoneNumberE164Regex();
}

public partial class WhenGenerateAge() : WhenGenerateSemantic<Age, int>(
    100, a => a.Value.Is().not.LessThan(0).and.LessThan(120));

public abstract class WhenGenerateDerivedValueType<TValueType>(int maxDistinct) : Spec<TValueType>
{
    [Fact]
    public void GivenMaxDistinctValues_ThenAreDifferent()
        => Enumerable.Range(1, maxDistinct).Select(n => Any<TValueType>()).Is().Distinct();
}

public partial class WhenGenerateMyEmail : WhenGenerateDerivedValueType<MyEmail>
{
    private static readonly Regex _emailRegex = EmailRegex();
    public WhenGenerateMyEmail() : base(100) => Register<MyEmail>().As<Email>();

    [Fact] public void ThenHasValidEmailFormat() => An<MyEmail>().Value.Has(e => _emailRegex.IsMatch(e));

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

public partial class WhenGenerateMyPhoneNumber : WhenGenerateDerivedValueType<MyPhoneNumber>
{
    private static readonly Regex _phoneNumberE164Regex = PhoneNumberE164Regex();
    public WhenGenerateMyPhoneNumber() : base(100) => Register<MyPhoneNumber>().As<PhoneNumber>();

    [Fact] public void ThenHasE164Format() => A<MyPhoneNumber>().Value.Has(p => _phoneNumberE164Regex.IsMatch(p));

    [GeneratedRegex(@"^\+[1-9]\d{1,14}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PhoneNumberE164Regex();
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

public record MyEmail(string Value);
public record MyPhoneNumber(string Value);