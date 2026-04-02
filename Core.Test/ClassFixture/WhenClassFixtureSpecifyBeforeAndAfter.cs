using Xspec.Assert;
using Xspec.Test.AutoMock;

namespace Xspec.Test.ClassFixture;

public static class WhenClassFixtureSpecifyBeforeAndAfter
{
    public class WhenGetValues : Spec<MyWrapper<int>, (int, int)>
    {
        internal int _setupCount = 0;

        public WhenGetValues()
            => When(_ => _.GetValues(An<int>()))
            .After(_ => _setupCount++)
            .Before(_ =>
            {
                Xunit.Assert.Equal(1, _setupCount);
                An<int>(_ => 999);
            });
    }

    public class GivenOneAndTwo(WhenGetValues classFixture) 
        : Spec<MyWrapper<int>, (int, int)>(classFixture), IClassFixture<WhenGetValues>
    {
        [Fact] public void ThenIsSetup() => Then(classFixture)._setupCount.Is(1);
        [Fact] public void ThenReturnOneAndTwo() => Given().Using(1).and.Default(2).Then().Result.Is((1, 2));
    }

    public class GivenThreeAndFour(WhenGetValues classFixture) 
        : Spec<MyWrapper<int>, (int, int)>(classFixture), IClassFixture<WhenGetValues>
    {
        [Fact] public void ThenReturnThreeAndFour() => Given().Using(3).and.Default(4).Then().Result.Is((3, 4));
        [Fact] public void ThenIsSetup() => Then(classFixture)._setupCount.Is(1);
    }
}