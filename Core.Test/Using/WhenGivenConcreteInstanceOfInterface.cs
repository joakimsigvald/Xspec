using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Using;

public class WhenUsingConcreteInstanceOfInterface : Spec<MyService, int>
{
    public WhenUsingConcreteInstanceOfInterface() => When(_ => _.GetNextId());

    [Fact]
    public void WithoutCast_ThenUseIt()
    {
        Given().Using(new FakeRepository(An<int>()))
            .Then().Result.Is(The<int>());
    }

    [Fact]
    public void WithDifferentCast_ThenDoNotUseIt()
    {
        Given().Using<object>(new FakeRepository(An<int>()))
            .Then().Result.Is().Not(The<int>());
    }

    [Fact]
    public void WithCast_ThenUseIt()
    {
        Given().Using<IMyRepository>(new FakeRepository(An<int>()))
            .Then().Result.Is(The<int>());
    }

    [Fact]
    public void GivenConcreteTypeArg_ThenUseIt()
    {
        Given().Using<FakeRepository>().and.Using(123).Then().Result.Is(123);
        Specification.Is(
            """
            Given using 123
              and using FakeRepository
            When _.GetNextId()
            Then Result is 123
            """
            );
    }
}

public class FakeRepository(int fakeId) : IMyRepository
{
    public int[] GetIds() => [];
    public MyModel GetModel() => new();
    public MyModel[] GetModels() => [];
    public Task<IEnumerable<MyModel>> GetModelsAsync() => Task.FromResult(Enumerable.Empty<MyModel>());
    public int GetNextId() => fakeId;
    public MyModel SetModel(MyModel model) => model;
}