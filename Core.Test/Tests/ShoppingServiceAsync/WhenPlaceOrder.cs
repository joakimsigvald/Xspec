using Xspec.Assert;
using Xspec.Test.Subjects;

namespace Xspec.Test.Tests.ShoppingServiceAsync;

public abstract class WhenPlaceOrder : ShoppingServiceAsyncSpec<object>
{
    protected ShoppingCart Cart;

    protected WhenPlaceOrder() => When(_ => _.PlaceOrder(Cart));

    public class GivenOpenCart : WhenPlaceOrder
    {
        public GivenOpenCart() => Using(() => Cart = new() { IsOpen = true });

        [Fact]
        public void ThenOrderIsCreated()
        {
            Then<IOrderService>(_ => _.CreateOrder(Cart));
            Specification.Is(
                """
                Using Cart = new() { IsOpen = true }
                When _.PlaceOrder(Cart)
                Then IOrderService.CreateOrder(Cart)
                """);
        }
    }

    public class GivenClosedCart : WhenPlaceOrder
    {
        public GivenClosedCart() => Using(() => Cart = new() { IsOpen = false });

        [Fact]
        public void ThenThrowsNotPurchasable()
        {
            Then().Throws<NotPurchasable>();
            Specification.Is(
                """
                Using Cart = new() { IsOpen = false }
                When _.PlaceOrder(Cart)
                Then throws NotPurchasable
                """);
        }
    }
}