using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public class WhenGivenFunction : Spec<MyService, DateTime>
{
    [Fact]
    public void UsingFunction_CanBeUsedAsDefaultFunction()
    {
        Given(A<DateTime>).When(_ => _.GetTime())
            .Then().Result.Is(The<DateTime>());
        Specification.Is(
            """
            Given a DateTime
            When _.GetTime()
            Then Result is the DateTime
            """);
    }

    [Fact]
    public void UsingValue_CanBeUsedForDefaultFunction()
    {
        Given(A<DateTime>()).When(_ => _.GetTime())
            .Then().Result.Is(The<DateTime>());
        Specification.Is(
            """
            Given a DateTime
            When _.GetTime()
            Then Result is the DateTime
            """);
    }
}