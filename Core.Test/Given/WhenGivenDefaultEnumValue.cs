using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public class WhenGivenDefaultEnumValue : Spec<MyService, MyEnum>
{
    public WhenGivenDefaultEnumValue() => When(_ => MyService.Echo(The<MyEnum>())).Given(MyEnum.Two);
    [Fact]
    public void ThenUseDefaultValue()
    {
        Result.Is(MyEnum.Two);
        Specification.Is(
            """
            Given MyEnum.Two
            When MyService.Echo(the MyEnum)
            Then Result is MyEnum.Two
            """);
    }
}