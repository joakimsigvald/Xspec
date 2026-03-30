using Xspec.Assert;

namespace Xspec.Test.Assert;

public class WhenList : Spec<List<string>>
{
    [Fact]
    public void IsEqual()
    {
        var values = new[] { "a", "b" };
        var list1 = values.ToList();
        var list2 = values.ToList();
        list1.Is().EqualTo(list2);
        Specification.Is("List1 is equal to list2");
    }

    [Fact]
    public void IsLike()
    {
        var values = new[] { "a", "b" };
        var list1 = values.ToList();
        var list2 = values.Reverse();
        list1.Is().Like(list2);
        Specification.Is("List1 is like list2");
    }

    [Fact]
    public void IsEquivalentTo()
    {
        var values = new[] { "a", "b" };
        var list1 = values.ToList();
        var list2 = values.Reverse();
        list1.Is().EquivalentTo(list2);
        Specification.Is("List1 is equivalent to list2");
    }

    [Fact]
    public void ChainAssertions()
    {
        When(_ => ["1", "2", "3"])
        .Then().Result.Is().not.Null().and.not.Empty()
        .and.Has().All((it, i) => it.Is($"{i + 1}"));
    }
}