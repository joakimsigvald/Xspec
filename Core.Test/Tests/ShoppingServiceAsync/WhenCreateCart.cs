using Xspec.Assert;
using Xspec.Test.Subjects;

namespace Xspec.Test.Tests.ShoppingServiceAsync;

public abstract class WhenCreateCart : ShoppingServiceAsyncSpec<ShoppingCart>
{
    protected int Id;

    protected WhenCreateCart() => When(_ => _.CreateCart(Id));

    public class GivenIdIsOne : WhenCreateCart
    {
        public GivenIdIsOne() => Given(() => Id = 1);

        [Fact]
        public void ThenCartIdIsOne()
        {
            Result.Id.Is(Id);
            Specification.Is(
                """
                Given Id = 1
                When _.CreateCart(Id)
                Then Result.Id is Id
                """);
        }
    }

    public class GivenIdIsTwo : WhenCreateCart
    {
        public GivenIdIsTwo() => Given(() => Id = 2);

        [Fact]
        public void ThenCartIdIsTwo()
        {
            Result.Id.Is(Id);
            Specification.Is(
                """
                Given Id = 2
                When _.CreateCart(Id)
                Then Result.Id is Id
                """);
        }
    }
}