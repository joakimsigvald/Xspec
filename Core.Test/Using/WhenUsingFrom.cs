using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Using;

public class WhenUsingFrom : Spec<MyService, int>
{
    public WhenUsingFrom() => When(_ => _.GetNextId());

    [Fact]
    public void GivenFrom_ThenConcreteClassIsNotUsed()
    {
        Using<MarkerRepository>().From<int>().And(123, For.Subject).Then().Result.Is().Not(42);
        Specification.Is(
            """
            Using MarkerRepository from int
              and 123 for Subject
            When _.GetNextId()
            Then Result is not 42
            """
            );
    }
}

public class MarkerRepository : IMyRepository
{
    public int[] GetIds() => [];
    public MyModel GetModel() => new();
    public MyModel[] GetModels() => [];
    public Task<IEnumerable<MyModel>> GetModelsAsync() => Task.FromResult(Enumerable.Empty<MyModel>());
    public int GetNextId() => 42;
    public MyModel SetModel(MyModel model) => model;
}

public class WhenChainUsingFrom : Spec<int>
{
    public WhenChainUsingFrom() => Using<int>().From<byte>().And<long>().From<short>();

    [Fact]
    public void ThenBothConversionsApply()
    {
        Three<int>().Is().EqualTo([1, 2, 3]);
        Three<long>().Is().EqualTo([4, 5, 6]);
    }
}

#pragma warning disable CS0618 // verify that the obsolete two-type-param forms still work
public class WhenUsingObsoleteTwoTypeParamForm : Spec<int>
{
    public WhenUsingObsoleteTwoTypeParamForm() => Using<int, byte>().And<long, short>();

    [Fact]
    public void ThenConversionsStillApply()
    {
        Three<int>().Is().EqualTo([1, 2, 3]);
        Three<long>().Is().EqualTo([4, 5, 6]);
    }
}
#pragma warning restore CS0618

public class WhenFromTwice : Spec<MyService, int>
{
    [Fact]
    public void ThenThrowsSetupFailed()
    {
        var continuation = Using<FakeRepository>();
        continuation.From<int>();
        Xunit.Assert.Throws<SetupFailed>(() => continuation.From<string>());
    }
}

public class WhenFromAfterSetup : Spec<MyService, int>
{
    public WhenFromAfterSetup() => When(_ => _.GetNextId());

    [Fact]
    public void ThenThrowsSetupFailed()
    {
        var continuation = Using<FakeRepository>();
        continuation.Then();
        Xunit.Assert.Throws<SetupFailed>(() => continuation.From<int>());
    }
}
