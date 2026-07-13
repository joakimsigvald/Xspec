using Xspec.Assert;
using Xspec.Internal.Specification;

namespace Xspec.Test.Internal.Specification.ArchDescriber;

public class WhenDescribeSelector : Spec<string>
{
    [Theory]
    [InlineData("_ => _.References", "References")]
    [InlineData("_ => _.PackageReferences", "PackageReferences")]
    [InlineData("_ => _.SourceFilesExcept(\"Program.cs\")", "SourceFilesExcept(\"Program.cs\")")]
    public void ThenReturnDescription(string selectExpr, string expected)
        => When(_ => selectExpr.DescribeSelector())
        .Then().Result.Is(expected);
}
