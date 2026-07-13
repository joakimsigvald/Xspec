using Xspec.Assert;
using Xspec.Internal.Specification;

namespace Xspec.Test.Internal.Specification.ArchDescriber;

public class WhenDescribePredicate : Spec<string>
{
    [Theory]
    [InlineData("_ => _.IsContract()", "are contracts")]
    [InlineData("_ => _.IsHost", "are hosts")]
    [InlineData("_ => _.HasDependency()", "have dependencies")]
    [InlineData("_ => !_.IsHost()", "are not hosts")]
    [InlineData("_ => _.IsHost() || _.IsTest()", "IsHost() or IsTest()")]
    [InlineData("_ => _.IsHost() && _.IsTest()", "IsHost() and IsTest()")]
    [InlineData("_ => _.InLayer(\"Domain\")", "InLayer(\"Domain\")")]
    public void ThenReturnDescription(string filterExpr, string expected)
        => When(_ => filterExpr.DescribePredicate())
        .Then().Result.Is(expected);
}
