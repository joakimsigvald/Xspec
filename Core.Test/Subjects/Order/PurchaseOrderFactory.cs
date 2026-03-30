using Xspec.Test.Subjects.Purchase;
using Xspec.Test.Subjects.Shopping;

namespace Xspec.Test.Subjects.Order;

public class PurchaseOrderFactory
{
    private readonly IBasketItemFactory _basketItemFactory;

    public PurchaseOrderFactory(IBasketItemFactory _basketItemFactory)
        => this._basketItemFactory = _basketItemFactory;

    public async Task<OrderRecord> CreateOrder(Checkout checkout)
    {
        await GetBasketItems(checkout.Basket);
        return new()
        {
            QuotationId = checkout.Basket.Id,
            OrderNo = $"{checkout.Basket.Id}"
        };
    }

    private async Task<BasketItem[]> GetBasketItems(Basket basket)
    {
        return await _basketItemFactory.CreateBasketItems(basket.CustomerId, basket.CompanyId);
    }
}