using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public class WhenGivenValue : Spec<MyService, MyModel>
{
    [Fact]
    public void AsFirstSentence_ThenValueInPipeline()
    {
        Given(() => new MyModel() { Name = A<string>() })
            .When(_ => MyService.Echo(The<MyModel>()))
            .Then().Result.Name.Is(The<string>());
        Specification.Is(
            """
            Given new MyModel() { Name = A<string>() }
            When MyService.Echo(the MyModel)
            Then Result.Name is the string
            """);
    }

    [Fact]
    public void AsSecondSentence_ThenUseValueInPipeline()
    {
        Given<IMyRepository>().That(_ => _.GetModel()).Returns(A<MyModel>)
            .And(() => new MyModel() { Name = A<string>() })
            .When(_ => _.GetModel())
            .Then().Result.Name.Is(The<string>());
        Specification.Is(
            """
            Given new MyModel() { Name = A<string>() }
              and IMyRepository.GetModel() returns a MyModel
            When _.GetModel()
            Then Result.Name is the string
            """);
    }

    [Fact]
    public void AsReturnsValue_ThenUseValueInPipeline()
    {
        Given<IMyRepository>().That(_ => _.GetModel()).Returns(() => A(new MyModel() { Name = A<string>() }))
            .When(_ => _.GetModel())
            .Then().Result.Name.Is(The<string>());
        Specification.Is(
            """
            Given IMyRepository.GetModel() returns a new MyModel() { Name = A<string>() }
            When _.GetModel()
            Then Result.Name is the string
            """);
    }

    [Fact]
    public void GivenNull_ThenUseNullInPipeline()
    {
        Given<IMyRepository>().That(_ => _.GetModel()).Returns(() => A<MyModel>())
            .And((MyModel)null)
            .When(_ => _.GetModel()).Then().Result.Is().Null();
        Specification.Is(
            """
            Given (MyModel)null
              and IMyRepository.GetModel() returns a MyModel
            When _.GetModel()
            Then Result is null
            """);
    }
}