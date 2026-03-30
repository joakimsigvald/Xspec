using Xspec.Test.Subjects.Shopping;

namespace Xspec.Test.Subjects.Purchase;

public interface IBasketItemFactory
{
    Task<BasketItem[]> CreateBasketItems(int customerId, int companyId);
}