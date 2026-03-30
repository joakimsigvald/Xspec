using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public class GivenDefaultEnumerableNotProvidedWhenGetTaskOfIEnumerable : Spec<MyService, MyModel[]>
{
    [Fact]
    public void GivenDefaultEnumerableNotProvided_WhenGetTaskOfEnumerable_ThrowSetupFailed()
    {
        Xunit.Assert.Throws<SetupFailed>(() =>
        When(_ => _.GetModelsAsync()).Then().DoesNotThrow());
    }
}