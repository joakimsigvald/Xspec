using Xspec.Assert;

namespace Xspec.Test.AutoMock;

public class WhenSubjectIsString : Spec<string>
{
    [Fact]
    public void ThenUseDefaultString()
    {
        Given("abc").When(_ => _).Then().Result.Is("abc");
        Specification.Is(
            """
            Given "abc"
            When _
            Then Result is "abc"
            """);
    }
}