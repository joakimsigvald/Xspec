using Xspec.Assert;

namespace Xspec.Test.AutoFixture;

public class WhenAnyNumberOf : Spec<MyRetriever, MyModel[]>
{
    public WhenAnyNumberOf() => Given(AnyNumberOf<MyModel>).When(_ => _.List());

    public class GivenNoOtherReference : WhenAnyNumberOf
    {
        [Fact]
        public void ThenArrayHasOneElements()
        {
            Result.Has().Count(1);
            Specification.Is(
                """
                Given any number of MyModel
                When _.List()
                Then Result has count 1
                """);
        }
    }

    public class GivenZeroIsMentionedBefore : WhenAnyNumberOf
    {
        public GivenZeroIsMentionedBefore() => Given(Zero<MyModel>).And(AnyNumberOf<MyModel>);

        [Fact]
        public void ThenCountIsZero()
        {
            Result.Has().Count(0);
            Specification.Is(
                """
                Given any number of MyModel
                  and zero MyModel
                  and any number of MyModel
                When _.List()
                Then Result has count 0
                """);
        }
    }
}
