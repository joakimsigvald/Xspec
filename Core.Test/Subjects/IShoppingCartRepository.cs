namespace Xspec.Test.Subjects;

public interface IShoppingCartRepository
{
    Task<ShoppingCart> GetCart(int id);
    Task StoreCart(ShoppingCart cart);
}