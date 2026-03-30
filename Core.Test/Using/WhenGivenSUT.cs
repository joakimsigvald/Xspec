using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Using;

public class WhenGivenSUT : Spec<MyService, (int, string)>
{
    public WhenGivenSUT()
        => Given(new MyService(An<IMyRepository>(), new MySettings { ConnectionString = A<string>() }, () => DateTime.Now))
            .And<IMyRepository>().That(_ => _.GetNextId()).Returns(An<int>)
        .When(_ => new(_.GetNextId(), _.GetConnectionString()));

    [Fact]
    public void ThenUseSUT()
    {
        Then().Result.Is((The<int>(), The<string>()));
        Specification.Is(
            """
            Given new MyService(an IMyRepository, new MySettings { ConnectionString = a
                  string }, DateTime.Now)
              and IMyRepository.GetNextId() returns an int
            When new(_.GetNextId(), _.GetConnectionString())
            Then Result is (the int, the string)
            """);
    }
}