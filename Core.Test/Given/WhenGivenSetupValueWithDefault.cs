using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public class WhenGivenSetupValueWithDefault : Spec<MyService, int>
{
    private const int _defaultId = 1;

    [Fact]
    public void GivenDefaultNotOverridden()
    {
        Given(_defaultId)
            .And<IMyRepository>().That(_ => _.GetNextId()).Returns(() => ASecond<int>())
            .When(_ => _.GetNextId())
            .Then().Result.Is(_defaultId);
        Specification.Is(
            """
            Given _defaultId
              and IMyRepository.GetNextId() returns a second int
            When _.GetNextId()
            Then Result is _defaultId
            """);
    }

    [Fact]
    public void GivenDefaultIsOverridden()
    {
        Given<IMyRepository>().That(_ => _.GetNextId()).Returns(() => ASecond<int>())
            .When(_ => _.GetNextId())
            .Given(_defaultId)
            .and.ASecond(2)
            .Then().Result.Is(2);
        Specification.Is(
            """
            Given _defaultId
              and a second int is 2
              and IMyRepository.GetNextId() returns a second int
            When _.GetNextId()
            Then Result is 2
            """);
    }
}