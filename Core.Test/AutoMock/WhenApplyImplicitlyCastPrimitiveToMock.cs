using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.AutoMock;

public class WhenApplyImplicitlyCastPrimitiveToMock : Spec<MyValueIntService, object>
{
    public WhenApplyImplicitlyCastPrimitiveToMock() => When(_ => _.SetValue(A<MyValueInt>()));

    [Fact]
    public void Then_ItIsApplied()
    {
        Then<IMyValueIntRepo>(_ => _.Set(The<MyValueInt>()));
        Specification.Is(
@"When _.SetValue(a MyValueInt)
Then IMyValueIntRepo.Set(the MyValueInt)");
    }
}