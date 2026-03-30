using Xspec.Assert;

namespace Xspec.Test.AutoFixture;

public class WhenSomeOtherWithTransform : Spec<MyRetriever, MyModel[]>
{
    public WhenSomeOtherWithTransform()
        => Given(() => SomeOther<MyModel>(m => m with { Id = The<int>() }, An<int>(i => 1 + i % 10)))
        .When(_ => _.List());

    [Fact]
    public void ThenArrayHasElementsWithSetup()
    {
        Result.Has().Count(The<int>());
        Result.Has().All(m => m.Id == The<int>());
        Specification.Is(
            $$$"""
            Given some other MyModel { m with { Id = The<int>() }, An<int>(i => 1 + i % 10)
                  }
            When _.List()
            Then Result has count 'the int' = {{{The<int>()}}}
            Result has all m.Id == The<int>()
            """);
    }
}