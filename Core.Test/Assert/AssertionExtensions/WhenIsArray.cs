using Xspec.Assert;

namespace Xspec.Test.Assert.AssertionExtensions;

public class WhenIsArray : Spec<int[]>
{
    public WhenIsArray() => Given().Default(() => new int[] { 1, 2, 3 });

    [Fact] public void GivenSame_ThenDoesNotThrow() => When(_ => _.Is(_)).Then();

    [Fact]
    public void GivenFail_ThenGetException()
    {
        var ex = Xunit.Assert.Throws<Xunit.Sdk.XunitException>(()
            => When(_ => _).Then().Result.Is([1, 2, 3]));
        ex.HasMessage(
            "Expected Result to be [1, 2, 3] but found [1, 2, 3]",
            """
            Given new int[] { 1, 2, 3 } is default
            When _
            Then Result is [1, 2, 3]
            """);
    }
}