using Xspec.Assert;

namespace Xspec.Test.AutoFixture;

public class WhenSomeOtherWithIndexedSetup : Spec<MyRetriever, MyModel[]>
{
    public WhenSomeOtherWithIndexedSetup()
        => Given(() => SomeOther<MyModel>((m, i) => m.Id = i, An<int>(i => 1 + i % 10)))
        .When(_ => _.List());

    [Fact]
    public void ThenArrayHasElementsWithSetup()
    {
        Result.Has().Count(The<int>());
        Result.Has().All((m, i) => m.Id == i);
        Specification.Is(
            $$$"""
            Given some other MyModel { Id = i, An<int>(i => 1 + i % 10) }
            When _.List()
            Then Result has count 'the int' = {{{The<int>()}}}
            Result has all Id = i
            """);
    }
}