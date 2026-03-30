using Xspec.Assert;
using Xspec.Test.Assert.Continuations.IsObject;

namespace Xspec.Test.Assert.AssertionExtensions;

public class WhenSatisfies : Spec
{
    [Fact]
    public void GivenTrue_ThenDoesNotThrow()
    {
        MyRecord actual = new("Abc");
        actual.Has(_ => _.Name == "Abc").and.Has(_ => _.Name == "Abc").and.Is().not.Null();
    }

    [Fact]
    public void GivenFalse_ThenGetException()
    {
        MyRecord myRecord = new("XXX");
        var ex = Xunit.Assert.Throws<Xunit.Sdk.XunitException>(() => myRecord.Has(_ => _.Name == "Abc"));
        ex.HasMessage($"Expected myRecord to have _.Name == \"Abc\" but found {myRecord}",
            "MyRecord has _.Name == \"Abc\"");
    }
}