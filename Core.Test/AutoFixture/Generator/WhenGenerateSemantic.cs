using System.Text.RegularExpressions;
using Xspec.Assert;
using Xspec.Semantic;

namespace Xspec.Test.AutoFixture.Primitives;

public abstract class WhenGenerateSemantic<TSemantic, TPrimitive>(int maxDistinct) : Spec<TSemantic>
    where TSemantic : Semantic<TPrimitive>, ISemantic<TPrimitive>
{
    [Fact]
    public void GivenSUT_ThenIsNotDefault()
        => When(_ => _).Then().Result.Value.Is().Not(default(TSemantic));

    [Fact]
    public void GivenMaxDistinctValues_ThenAreDifferent()
        => Enumerable.Range(1, maxDistinct).Select(n => Any<TSemantic>()).Is().Distinct();

    [Fact]
    public void ThenImplicitlyCastsToPrimitive() 
        => A<TSemantic>().Value.Is((TPrimitive)The<TSemantic>());
}

public partial class WhenGenerateEmail() : WhenGenerateSemantic<Email, string>(100)
{
    private static readonly Regex _emailRegex = EmailRegex();

    [Fact] public void ThenHasValidEmailFormat() => An<Email>().Has(e => _emailRegex.IsMatch(e));

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

public partial class WhenGeneratePhoneNumber() : WhenGenerateSemantic<PhoneNumber, string>(100)
{
    private static readonly Regex _phoneNumberE164Regex = PhoneNumberE164Regex();

    [Fact] public void ThenHasE164Format() => A<PhoneNumber>().Has(p => _phoneNumberE164Regex.IsMatch(p));

    [GeneratedRegex(@"^\+[1-9]\d{1,14}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PhoneNumberE164Regex();
}

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

    [Fact] public void ThenHasValidEmailFormat() => An<Email>().Has(e => _emailRegex.IsMatch(e));

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

public partial class WhenGenerateMyPhoneNumber : WhenGenerateDerivedValueType<MyPhoneNumber>
{
    private static readonly Regex _phoneNumberE164Regex = PhoneNumberE164Regex();
    public WhenGenerateMyPhoneNumber() : base(100) => Register<MyPhoneNumber>().As<PhoneNumber>();

    [Fact] public void ThenHasE164Format() => A<PhoneNumber>().Has(p => _phoneNumberE164Regex.IsMatch(p));

    [GeneratedRegex(@"^\+[1-9]\d{1,14}$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PhoneNumberE164Regex();
}

public record MyEmail(string Value);
public record MyPhoneNumber(string Number);