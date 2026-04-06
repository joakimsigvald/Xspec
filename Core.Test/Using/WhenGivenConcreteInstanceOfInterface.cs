using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Using;

public class WhenUsingConcreteInstanceOfInterface : Spec<MyService, int>
{
    public WhenUsingConcreteInstanceOfInterface() => When(_ => _.GetNextId());

    [Fact]
    public void WithoutCast_ThenUseIt()
    {
        Using(new FakeRepository(An<int>()), Scope.Subject)
            .Then().Result.Is(The<int>());
    }

    [Fact]
    public void WithDifferentCast_ThenDoNotUseIt()
    {
        Using<object>(new FakeRepository(An<int>()), Scope.Subject)
            .Then().Result.Is().Not(The<int>());
    }

    [Fact]
    public void WithCast_ThenUseIt()
    {
        Using<IMyRepository>(new FakeRepository(An<int>()), Scope.Subject)
            .Then().Result.Is(The<int>());
    }

    [Fact]
    public void GivenConcreteTypeArg_ThenUseIt()
    {
        Given().Using<FakeRepository>().Using(123, Scope.Subject).Then().Result.Is(123);
        Specification.Is(
            """
            Using 123 for Subject
            Given using FakeRepository
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