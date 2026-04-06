using Xspec.Assert;

namespace Xspec.Test.Given;

public class WhenGivenStaticPrimitive : Spec<string>
{
    [Theory]
    [InlineData("abc")]
    public void GivenDefaultValue_ThenUseDefault(string value)
    {
        Given().Default(value).When(_ => _).Then().Result.Is(value);
        Specification.Is(
            """
            Given value is default
            When _
            Then Result is value
            """);
    }

    [Theory]
    [InlineData("abc")]
    public void GivenDefaultSetup_ThenUseDefaultSetup(string value)
    {
        Given().Default<string>(_ => value).When(_ => _).Then().Result.Is(value);
        Specification.Is(
            """
            Given string is value
            When _
            Then Result is value
            """);
    }

    [Theory]
    [InlineData("abc", "def")]
    public void GivenDefaultAndAValue_ThenUseAValue(string defaultValue, string aValue)
    {
        Using(defaultValue, Scope.Input).Given().A(aValue)
            .When(_ => The<string>())
            .Then().Result.Is(aValue);
        Specification.Is(
            """
            Using defaultValue for Input
            Given a string is aValue
            When the string
            Then Result is aValue
            """);
    }

    [Theory]
    [InlineData("abc", "def")]
    public void GivenDefaultAndASecondValue_ThenUseTheSecondValue(string defaultValue, string aValue)
    {
        Using(defaultValue, Scope.Input).Given().ASecond(aValue)
            .When(_ => TheSecond<string>())
            .Then().Result.Is(aValue);
        Specification.Is(
            """
            Using defaultValue for Input
            Given a second string is aValue
            When the second string
            Then Result is aValue
            """);
    }

    [Theory]
    [InlineData("abc", "def")]
    public void GivenDefaultAndAThirdValue_ThenUseTheThirdValue(string defaultValue, string aValue)
    {
        Using(defaultValue, Scope.Input).Given().AThird(aValue)
            .When(_ => TheThird<string>())
            .Then().Result.Is(aValue);
        Specification.Is(
            """
            Using defaultValue for Input
            Given a third string is aValue
            When the third string
            Then Result is aValue
            """);
    }

    [Theory]
    [InlineData("abc", "def")]
    public void GivenDefaultAndAFourthValue_ThenUseTheFourthValue(string defaultValue, string aValue)
    {
        Using(defaultValue, Scope.Input).Given().AFourth(aValue)
            .When(_ => TheFourth<string>())
            .Then().Result.Is(aValue);
        Specification.Is(
            """
            Using defaultValue for Input
            Given a fourth string is aValue
            When the fourth string
            Then Result is aValue
            """);
    }

    [Theory]
    [InlineData("abc", "def")]
    public void GivenDefaultAndAFifthValue_ThenUseTheFifthValue(string defaultValue, string aValue)
    {
        Using(defaultValue, Scope.Input).Given().AFifth(aValue)
            .When(_ => TheFifth<string>())
            .Then().Result.Is(aValue);
        Specification.Is(
            """
            Using defaultValue for Input
            Given a fifth string is aValue
            When the fifth string
            Then Result is aValue
            """);
    }

    [Theory]
    [InlineData("abc")]
    public void GivenTransformSecondValue_ThenApplyTransformOn_TheSecondValue(string value)
    {
        Given().ASecond<string>(_ => value)
            .When(_ => TheSecond<string>())
            .Then().Result.Is(value);
        Specification.Is(
            """
            Given a second string is value
            When the second string
            Then Result is value
            """);
    }

    [Theory]
    [InlineData("abc")]
    public void GivenTransformThirdValue_ThenApplyTransformOn_TheThirdValue(string value)
    {
        Given().AThird<string>(_ => value)
            .When(_ => TheThird<string>())
            .Then().Result.Is(value);
        Specification.Is(
            """
            Given a third string is value
            When the third string
            Then Result is value
            """);
    }

    [Theory]
    [InlineData("abc")]
    public void GivenTransformFourthValue_ThenApplyTransformOn_TheFourthValue(string value)
    {
        Given().AFourth<string>(_ => value)
            .When(_ => TheFourth<string>())
            .Then().Result.Is(value);
        Specification.Is(
            """
            Given a fourth string is value
            When the fourth string
            Then Result is value
            """);
    }

    [Theory]
    [InlineData("abc")]
    public void GivenTransformFifthValue_ThenApplyTransformOn_TheFifthValue(string value)
    {
        Given().AFifth<string>(_ => value)
            .When(_ => TheFifth<string>())
            .Then().Result.Is(value);
        Specification.Is(
            """
            Given a fifth string is value
            When the fifth string
            Then Result is value
            """);
    }
}