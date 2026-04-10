using Xspec.Assert;
using Xspec.Test.Subjects;

namespace Xspec.Test.Tests.ShoppingServiceAsync;

public abstract class WhenRemoveItem : ShoppingServiceAsyncSpec<ShoppingCart>
{
    protected int CartId = 123;
    protected ShoppingCartItem[] CartItems;
    protected readonly ShoppingCartItem Item = new("X");
    private ShoppingCart _cart;

    protected WhenRemoveItem()
        => When(_ => _.RemoveFromCart(CartId, Cart.Items[0]))
        .Given<IShoppingCartRepository>().That(_ => _.GetCart(CartId)).Returns(() => new ShoppingCart { Id = CartId, Items = CartItems });

    protected ShoppingCart Cart => _cart ??= new() { Id = CartId, Items = CartItems };

    public class GivenCartWithOneItem : WhenRemoveItem
    {
        public GivenCartWithOneItem() => Using(() => CartItems = [new ShoppingCartItem("X")]);

        [Fact]
        public void ThenCartIsEmpty()
        {
            Result.Items.Is().Empty();
            Specification.Is(
                """
                Using CartItems = [new ShoppingCartItem("X")]
                Given IShoppingCartRepository.GetCart(CartId) returns new ShoppingCart { Id =
                      CartId, Items = CartItems }
                When _.RemoveFromCart(CartId, Cart.Items[0])
                Then Result.Items is empty
                """);
        }
    }
}