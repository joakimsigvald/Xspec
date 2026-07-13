using Xspec.Architecture;
using Xspec.Assert;

namespace Xspec.Test.Architecture;

public class WhenFabricateSolution : ArchSpec
{
    private static readonly Solution _solution = new(
    [
        new() { Name = "App.Contracts", RelativePath = "Contracts/App.Contracts/App.Contracts.csproj" },
        new()
        {
            Name = "App.Domain",
            ProjectReferences = ["App.Contracts"],
            PackageReferences = ["FluentValidation"],
            RootNamespace = "App",
        },
    ]);

    public WhenFabricateSolution() : base(_solution) { }

    [Fact]
    public void ThenProjectIsRetrievedByName()
        => Solution["App.Domain"].Name.Is("App.Domain");

    [Fact]
    public void GivenUnknownProjectName_ThenSetupFailed()
    {
        var ex = Xunit.Assert.Throws<SetupFailed>(() => Solution["Unknown"]);
        ex.Message.Is("Solution contains no project named 'Unknown'");
    }

    [Fact]
    public void GivenDuplicateProjectNames_ThenSetupFailed()
    {
        var ex = Xunit.Assert.Throws<SetupFailed>(() => new Solution(
        [
            new() { Name = "App", RelativePath = "a/App.csproj" },
            new() { Name = "App", RelativePath = "b/App.csproj" },
        ]));
        ex.Message.Is("Solution contains two projects named 'App' (a/App.csproj, b/App.csproj)");
    }

    [Fact]
    public void ThenDirectoryIsDerivedFromRelativePath()
        => Projects[0].Directory.Is("Contracts/App.Contracts");

    [Fact]
    public void GivenNoRelativePath_ThenDirectoryIsEmpty()
        => Projects[1].Directory.Is("");

    [Fact]
    public void ThenReferencesCombineProjectAndPackageReferences()
        => Projects[1].References.Is().EqualTo(["App.Contracts", "FluentValidation"]);

    [Fact]
    public void GivenNoRootNamespace_ThenRootNamespaceDefaultsToName()
        => Projects[0].RootNamespace.Is("App.Contracts");

    [Fact]
    public void GivenRootNamespace_ThenItIsKept()
        => Projects[1].RootNamespace.Is("App");
}
