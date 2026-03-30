using Xspec.Assert;
using Xspec.Test.TestData;

namespace Xspec.Test.Given;

public abstract class WhenGivenArrayOfModelsAsync : Spec<MyService, MyModel[]>
{
    protected WhenGivenArrayOfModelsAsync() => When(_ => _.GetModelsAsync());

    public class GivenDefaultEnumerableProvided : WhenGivenArrayOfModelsAsync
    {
        public GivenDefaultEnumerableProvided()
            => Given<IMyRepository>().Returns(An<IEnumerable<MyModel>>);

        [Fact]
        public void ThenCanGetTaskOfEnumerable()
        {
            Then().DoesNotThrow();
            Specification.Is(
                """
            Given IMyRepository returns an IEnumerable<MyModel>
            When _.GetModelsAsync()
            Then does not throw
            """);
        }
    }
}