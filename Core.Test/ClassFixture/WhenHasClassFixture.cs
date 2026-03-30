using Xspec.Assert;
using Xspec.Test.AutoMock;

namespace Xspec.Test.ClassFixture;

public class WhenHasClassFixture(MyClassFixture fixture)
    : Spec<MyService, int>(fixture), IClassFixture<MyClassFixture>
{
    [Fact]
    public void ThenSetupIsRunOnceBeforeFirstTest()
    {
        When(_ => _.GetValue()).Then().Result.Is(The<int>() + 1);
        Specification.Is(
            """
            Given _component = new MyComponent(an IMyLogger, a 1)
            When _.GetValue()
            After _component.SetValue(_component.GetValue() + 1)
            Before _component.SetValue(0)
            Then Result is the int + 1
            """);
    }

    [Fact]
    public void ThenSetupIsRunOnceBeforeSecondTest()
    {
        When(_ => _.GetValue()).Then().Result.Is(The<int>() + 1).and.Not(3);
        Specification.Is(
            """
            Given _component = new MyComponent(an IMyLogger, a 1)
            When _.GetValue()
            After _component.SetValue(_component.GetValue() + 1)
            Before _component.SetValue(0)
            Then Result is the int + 1
                and not 3
            """);
    }
}