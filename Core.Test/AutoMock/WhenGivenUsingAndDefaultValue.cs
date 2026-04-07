using Xspec.Assert;
using Xspec.Test.AutoFixture;

namespace Xspec.Test.AutoMock;

public class WhenGivenUsingAndDefaultValue : Spec<MyWrapper<string>, (string, string)>
{
    [Fact]
    public void ThenApplyBothAsValues()
    {
        Using("ABC", For.Input).And("DEF", For.Subject)
            .When(_ => _.GetValues(A<string>()))
            .Then().Result.Is(("DEF", "ABC"));
        Specification.Is(
            """
            Using "ABC" for Input
              and "DEF" for Subject
            When _.GetValues(a string)
            Then Result is ("DEF", "ABC")
            """);
    }

    [Fact]
    public void ThenApplyBothAsLambdas()
    {
        Using(() => "ABC", For.Input).And(() => "DEF")
            .When(_ => _.GetValues(A<string>()))
            .Then().Result.Is(("DEF", "ABC"));
        Specification.Is(
            """
            Using "DEF"
              and "ABC" for Input
            When _.GetValues(a string)
            Then Result is ("DEF", "ABC")
            """);
    }

    [Fact]
    public void UsingTag_ThenApplyBothAsLambdas()
    {
        Tag<string> def = new();
            Using(() => "ABC", For.Input)
            .Given().Using(def)
            .Given(def).Is("DEF")
            .When(_ => _.GetValues(A<string>()))
            .Then().Result.Is(("DEF", "ABC"));
        Specification.Is(
            """
            Using "ABC" for Input
            Given def is "DEF"
              and using def
            When _.GetValues(a string)
            Then Result is ("DEF", "ABC")
            """);
    }
}

public class WhenGivenUsingAndDefaultModel : Spec<MyWrapper<MyModel>, (MyModel, MyModel)>
{
    private readonly MyModel _first = new() { Id = 1};
    private readonly MyModel _second = new() { Id = 2 };

    [Fact]
    public void ThenApplyBothAsValues()
    {
        Using(_first, For.Input).And(_second, For.Subject)
            .When(_ => _.GetValues(A<MyModel>()))
            .Then().Result.Is((_second, _first));
        Specification.Is(
            """
            Using _first for Input
              and _second for Subject
            When _.GetValues(a MyModel)
            Then Result is (_second, _first)
            """);
    }

    [Fact]
    public void GivenTagsThenApplyBothAsValues()
    {
        Tag<MyModel> one = new(), two = new();
        Given().Default(one).and.Using(two)
            .Given(one).Is(_first).And(two).Is(_second)
            .When(_ => _.GetValues(A<MyModel>()))
            .Then().Result.Is((_second, _first));
        Specification.Is(
            """
            Given two is _second
              and one is _first
              and using two
              and one is default
            When _.GetValues(a MyModel)
            Then Result is (_second, _first)
            """);
    }

    [Fact]
    public void ThenApplyBothAsLambdas()
    {
        Using(() => _first, For.Input).And(() => _second, For.Subject)
            .When(_ => _.GetValues(A<MyModel>()))
            .Then().Result.Is((_second, _first));
        Specification.Is(
            """
            Using _second for Subject
              and _first for Input
            When _.GetValues(a MyModel)
            Then Result is (_second, _first)
            """);
    }
}