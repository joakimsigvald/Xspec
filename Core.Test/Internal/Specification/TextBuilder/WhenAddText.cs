using Xspec.Assert;

namespace Xspec.Test.Internal.Specification.TextBuilder;

public class WhenAddText : Spec<Xspec.Internal.Specification.TextBuilder, string>
{
    public WhenAddText()
    => Given(new Xspec.Internal.Specification.TextBuilder(10, 1))
    .When(_ => _.AddText(A<string>()).ToString());

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("123456789", "123456789")]
    [InlineData("1234567890", "1234567890")]
    [InlineData("12345678901",
        """
        1234567890
           1
        """)]
    [InlineData("123456789012345678901",
        """
        1234567890
           1234567
           8901
        """)]
    [InlineData("12345 678901",
        """
        12345
           678901
        """)]
    [InlineData("123 567 901",
        """
        123 567
           901
        """)]
    [InlineData("12345.78901",
        """
        12345.7890
           1
        """)]
    [InlineData("Abc[..7890]",
        """
        Abc[
           ..7890]
        """)]
    [InlineData("12345.A8901",
        """
        12345.
           A8901
        """)]
    [InlineData("ABC(567890)",
        """
        ABC(
           567890)
        """)]
    [InlineData("ABC[567890]",
        """
        ABC[
           567890]
        """)]
    [InlineData("ABC{567890}",
        """
        ABC{
           567890}
        """)]
    [InlineData("ABC<5678901>",
        """
        ABC<567890
           1>
        """)]
    public void ThenReturnDescription(string text, string expected)
        => Given(text).Then().Result.Is(expected);

    [Theory]
    [InlineData(null, null, "")]
    [InlineData("12345678", "901",
        """
        12345678
           901
        """)]
    public void GivenHasTextAndNextWordDoesNotFit_ThenBreakBeforeWord(
        string existingText, string newText, string expected)
        => After(_ => _.AddText(existingText)).Given(newText).Then().Result.Is(expected);
}