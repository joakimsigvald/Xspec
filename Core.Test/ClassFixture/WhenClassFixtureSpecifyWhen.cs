using Xspec.Assert;
using Xspec.Test.AutoMock;

namespace Xspec.Test.ClassFixture;

public static class WhenClassFixtureSpecifyWhen
{
    public class WhenGetValues : Spec<MyWrapper<int>, (int, int)>
    {
        public WhenGetValues() => When(_ => _.GetValues(An<int>()));
    }

    public class GivenOneAndTwo : Spec<MyWrapper<int>, (int, int)>, IClassFixture<WhenGetValues>
    {
        public GivenOneAndTwo(WhenGetValues _) : base(_)
            => Given().Using(1).and.Default(2);

        [Fact] public void ThenReturnOneAndTwo() => Result.Is((1, 2));
    }

    public class GivenThreeAndFour : Spec<MyWrapper<int>, (int, int)>, IClassFixture<WhenGetValues>
    {
        public GivenThreeAndFour(WhenGetValues _) : base(_) 
            => Given().Using(3).and.Default(4);

        [Fact] public void ThenReturnThreeAndFour() => Result.Is((3, 4));
    }
}