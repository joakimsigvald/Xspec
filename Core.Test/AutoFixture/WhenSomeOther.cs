using Xspec.Assert;

namespace Xspec.Test.AutoFixture;

public abstract class WhenSomeOther : Spec<MyRetriever, MyModel[]>
{
    public WhenSomeOther() => Using(() => SomeOther<MyModel>()).When(_ => _.List());

    public class GivenNoOtherReference : WhenSomeOther
    {
        [Fact]
        public void ThenArrayHasTwoElements()
        {
            Result.Has().Count(2);
            Specification.Is(
                """
                Using some other MyModel
                When _.List()
                Then Result has count 2
                """);
        }
    }

    public class GivenOneIsMentionedAfter : WhenSomeOther
    {
        public GivenOneIsMentionedAfter() => Using(One<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Using one MyModel
  and some other MyModel
When _.List()
Then Result has count 2");
        }

        [Fact]
        public void ThenFirstModelIsNotTheOneModel()
        {
            Result.First().Is().Not(TheFirst<MyModel>());
            Specification.Is(
@"Using one MyModel
  and some other MyModel
When _.List()
Then Result.First() is not the first MyModel");
        }
    }

    public class GivenThreeIsMentionedAfter : WhenSomeOther
    {
        public GivenThreeIsMentionedAfter() => Using(Three<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Using three MyModel
  and some other MyModel
When _.List()
Then Result has count 2");
        }
    }

    public class GivenOneIsMentionedBefore : WhenSomeOther
    {
        public GivenOneIsMentionedBefore() => Using(One<MyModel>).And(Some<MyModel>);

        [Fact]
        public void ThenCountIsTwo()
        {
            Result.Has().Count(2);
            Specification.Is(
@"Using some MyModel
  and one MyModel
  and some other MyModel
When _.List()
Then Result has count 2");
        }
    }

    public class GivenTwoIsMentionedBefore : WhenSomeOther
    {
        public GivenTwoIsMentionedBefore() => Using(Two<MyModel>).And(Some<MyModel>);

        [Fact]
        public void ThenTwoModelsAreNotThoseTwoModels()
        {
            Result.Is().not.Like(Two<MyModel>());
            Specification.Is(
@"Using some MyModel
  and two MyModel
  and some other MyModel
When _.List()
Then Result is not like two MyModel");
        }
    }

    public class GivenSpecificNumberOfOther : WhenSomeOther
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(10)]
        public void ThenGetThatNumberOfOtherElements(int count) 
        {
            Then();
            SomeOther<MyModel>(count).Length.Is(count);
            SomeOther<MyModel>().Is().not.Like(SomeOther<MyModel>());
        }
    }
}