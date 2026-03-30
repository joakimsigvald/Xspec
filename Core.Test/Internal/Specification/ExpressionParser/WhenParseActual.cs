using Xspec.Assert;
using Xspec.Internal.Specification;

namespace Xspec.Test.Internal.Specification.ExpressionParser;

public class WhenParseActual : Spec<string>
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Something().That", "")]
    [InlineData("Then().Result.Name", "Result.Name")]
    [InlineData("And(Result).Id", "Result.Id")]
    [InlineData("The<int>()", "the int")]
    public void ThenReturnDescription(string returnsExpr, string expected)
        => When(_ => returnsExpr.ParseActual()).Then().Result.Is(expected);

    [Theory]
    [InlineData("And(Result.Id)")]
    public void GivenInvalidSetup_ThenThrow(string returnsExpr)
        => Xunit.Assert.Throws<SetupFailed>(() => When(_ => returnsExpr.ParseActual()).Then());
}