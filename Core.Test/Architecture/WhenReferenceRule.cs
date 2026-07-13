using Xspec.Architecture;
using Xspec.Assert;
using Xunit.Sdk;

namespace Xspec.Test.Architecture;

public class WhenReferenceRule : ArchSpec
{
    private static readonly Solution _solution = new(
    [
        new() { Name = "App.Contracts" },
        new() { Name = "App.Domain", ProjectReferences = ["App.Contracts"] },
        new() { Name = "App.Host", ProjectReferences = ["App.Domain"], PackageReferences = ["Serilog"] },
    ]);

    public WhenReferenceRule() : base(_solution) { }

    [Fact]
    public void GivenNoOffendingReferences_ThenRulePassesAndSpecificationIsGenerated()
    {
        When(Projects, _ => _.IsContract())
            .Then(because: "contracts are the external surface")
            .They.HaveNo(_ => _.References);
        Specification.Is(
            """
            When Projects are contracts
            Then they have no References, because contracts are the external surface
            """);
    }

    [Fact]
    public void GivenOffendingReferences_ThenFailureListsEachViolationOnItsOwnLine()
    {
        var ex = Xunit.Assert.Throws<XunitException>(() =>
            When(Projects, _ => _.IsHost())
                .Then(because: "hosts compose modules")
                .They.HaveNo(_ => _.References));
        ex.HasMessage(
            """
            Expected Projects that are hosts to have no References but found 2 violations:
              App.Host: App.Domain
              App.Host: Serilog
            """,
            """
            When Projects are hosts
            Then they have no References, because hosts compose modules
            """);
    }

    [Fact]
    public void GivenNoFilter_ThenRuleAppliesToAllElements()
    {
        var ex = Xunit.Assert.Throws<XunitException>(() =>
            When(Projects)
                .Then(because: "packages are centralized")
                .They.HaveNo(_ => _.PackageReferences));
        ex.HasMessage(
            """
            Expected Projects to have no PackageReferences but found 1 violation:
              App.Host: Serilog
            """,
            """
            When Projects
            Then they have no PackageReferences, because packages are centralized
            """);
    }

    [Fact]
    public void GivenNoBecause_ThenSpecificationHasNoReason()
    {
        When(Projects, _ => _.IsContract()).Then().They.HaveNo(_ => _.ProjectReferences);
        Specification.Is(
            """
            When Projects are contracts
            Then they have no ProjectReferences
            """);
    }
}
