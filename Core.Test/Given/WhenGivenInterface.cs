using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public class WhenGivenInterface : Spec<MyService, string>
{
    [Fact]
    public void ThenUseValueInPipeline()
    {
        Given<IMySettings>(new MySettings { ConnectionString = ASecond<string>() })
            .When(_ => _.GetConnectionString())
            .Then().Result.Is(TheSecond<string>());
        Specification.Is(
            """
            Given new MySettings { ConnectionString = a second string }
            When _.GetConnectionString()
            Then Result is the second string
            """);
    }
}