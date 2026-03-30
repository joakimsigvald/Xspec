using Xspec.Assert;

namespace Xspec.Test.AutoFixture;

public class WhenSome : Spec<MyRetriever, MyModel[]>
{
    public WhenSome() => Given(Some<MyModel>).When(_ => _.List());

    public class GivenNoOtherReference : WhenSome
    {
        [Fact]
        public void ThenArrayHasTwoElements()
        {
            Result.Has().Count(2);
            Specification.Is(
                """
                Given some MyModel
                When _.List()
                Then Result has count 2
                """);
        }
    }

    public class GivenOneIsMentionedAfter : WhenSome
    {
        public GivenOneIsMentionedAfter() => Given(One<MyModel>);

        [Fact]
        public void ThenCountIsOne()
        {
            Result.Has().Count(1);
            Specification.Is(
@"Given one MyModel
  and some MyModel
When _.List()
Then Result has count 1");
        }
    }

    public class GivenThreeIsMentionedAfter : WhenSome
    {
        public GivenThreeIsMentionedAfter() => Given(Three<MyModel>);

        [Fact]
        public void ThenCountIsThree()
        {
            Result.Has().Count(3);
            Specification.Is(
@"Given three MyModel
  and some MyModel
When _.List()
Then Result has count 3");
        }
    }

    public class GivenEmptyIsMentionedAfter : WhenSome
    {
        public GivenEmptyIsMentionedAfter() => Given(Array.Empty<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Given Array.Empty<MyModel>
  and some MyModel
When _.List()
Then Result has count 2");
        }
    }

    public class GivenManyIsMentionedAfter : WhenSome
    {
        public GivenManyIsMentionedAfter() => Given(Many<MyModel>);

        [Fact]
        public void ThenCountIsFour()
        {
            Result.Has().Count(3);
            Specification.Is(
@"Given many MyModel
  and some MyModel
When _.List()
Then Result has count 3");
        }
    }

    public class GivenOneIsMentionedBefore : WhenSome
    {
        public GivenOneIsMentionedBefore() => Given(One<MyModel>).And(Some<MyModel>);

        [Fact]
        public void ThenCountIsOne()
        {
            Result.Has().Count(1);
            Specification.Is(
@"Given some MyModel
  and one MyModel
  and some MyModel
When _.List()
Then Result has count 1");
        }
    }

    public class GivenTwoIsMentionedBefore : WhenSome
    {
        public GivenTwoIsMentionedBefore() => Given(Two<MyModel>).And(Some<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Given some MyModel
  and two MyModel
  and some MyModel
When _.List()
Then Result has count 2");
        }
    }

    public class GivenEmptyIsMentionedBefore : WhenSome
    {
        public GivenEmptyIsMentionedBefore() => Given(Array.Empty<MyModel>).And(Some<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Given some MyModel
  and Array.Empty<MyModel>
  and some MyModel
When _.List()
Then Result has count 2");
        }
    }

    public class GivenManyIsMentionedBefore : WhenSome
    {
        public GivenManyIsMentionedBefore() => Given(Many<MyModel>).And(Some<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Given some MyModel
  and many MyModel
  and some MyModel
When _.List()
Then Result has count 2");
        }
    }
}