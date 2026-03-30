using Xspec.Assert;
using Xspec.Test.TestData;
using static Xspec.Test.Given.WhenGivenContainingRecord;

namespace Xspec.Test.Given;

public class WhenGivenContainingRecord : Spec<MyService, ContainingRecord>
{
    [Fact]
    public void GivenDefaultRecordSetupContainedInAnotherRecord_ThenUseDefault()
    {
        Given(new MyRecord(1, A<string>()))
            .When(_ => MyService.Echo(A<ContainingRecord>()))
            .Then().Result.MyRecord.Name.Is(The<string>());
    }

    [Fact]
    public void GivenDefaultRecordInstanceContainedInAnotherRecord_ThenUseDefault()
    {
        Given<MyRecord>(_ => _ with { Name = A<string>() })
            .When(_ => MyService.Echo(A<ContainingRecord>()))
            .Then().Result.MyRecord.Name.Is(The<string>());
    }

    public record ContainingRecord(MyRecord MyRecord);
}