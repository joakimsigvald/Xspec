using Xspec.Assert;

namespace Xspec.Test.Assert.AssertionExtensions;

public class WhenIsNotArray : Spec<int[]>
{
    public WhenIsNotArray() => Given().Default(() => new int[] { 1, 2, 3 });

    [Fact] public void GivenNotSame_ThenDoesNotThrow() => When(_ => _.Is().Not(null)).Then();

    [Fact]
    public void GivenFail_ThenGetException()
    {
        int[] arr = [1, 2];
        var ex = Xunit.Assert.Throws<Xunit.Sdk.XunitException>(()
            => When(_ => arr).Then().Result.Is().Not(arr));
        ex.HasMessage(
            "Expected Result to not be [1, 2] but found [1, 2]",
            """
            Given new int[] { 1, 2, 3 } is default
            When arr
            Then Result is not arr
            """);
    }
}