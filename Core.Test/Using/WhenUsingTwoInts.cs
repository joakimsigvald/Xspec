using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Using;

public class WhenUsingTwoInts : Spec<MyListService, List<int>>
{
    [Fact]
    public void ThenReturnTwoInts()
    {
        Using(Two<int>().ToList()).When(_ => _.List).Then().Result.Has().Count(2);
        Specification.Is(
            """
            Using two int's ToList()
            When _.List
            Then Result has count 2
            """);
    }
}