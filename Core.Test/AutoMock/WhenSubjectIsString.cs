using Xspec.Assert;

namespace Xspec.Test.AutoMock;

public class WhenSubjectIsString : Spec<string>
{
    [Fact]
    public void ThenUseDefaultString()
    {
        Using("abc").When(_ => _).Then().Result.Is("abc");
        Specification.Is(
            """
            Using "abc"
            When _
            Then Result is "abc"
            """);
    }
}