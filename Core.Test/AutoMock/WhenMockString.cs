using Xspec.Assert;

namespace Xspec.Test.AutoMock;

public class WhenMockString : Spec<StaticStringService, string>
{
    public WhenMockString() => Given(A<string>).When(_ => _.GetValue());
    public class UsingAString : WhenMockString
    {
        [Fact]
        public void Then_It_Has_TheString()
        {
            Then().Result.Is(The<string>());
            Specification.Is(
                """
                Given a string
                When _.GetValue()
                Then Result is the string
                """);
        }
    }

    public class GivenItWasProvided : WhenMockString
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("hej")]
        public void Then_It_Has_ProvidedValue(string value)
        {
            Given(value).Then().Result.Is(value);
            Specification.Is(
                """
                Given value
                  and a string
                When _.GetValue()
                Then Result is value
                """);
        }
    }
}

public class StaticStringService(string value)
{
    private readonly string _value = value;
    public string GetValue() => _value;
}