using Xspec.Test.Subjects;
using Moq;
using Xspec.Assert;

namespace Xspec.Test.Tests.ShoppingService;

public class WhenPlaceOrder : ShoppingServiceSpec<object>
{
    public WhenPlaceOrder() => When(_ => _.PlaceOrder(A<ShoppingCart>()));

    [Theory]
    [InlineData(2)]
    [InlineData(123)]
    public void ThenOrderIsCreated(int shopId)
    {
        Given(shopId)
            .Then<IOrderService>(_ => _.CreateOrder(The<ShoppingCart>()))
            .And<ILogger>(_ => _.ForContext("ShopId", shopId));
        Specification.Is(
            """
            Given shopId
            When _.PlaceOrder(a ShoppingCart)
            Then IOrderService.CreateOrder(the ShoppingCart)
              and ILogger.ForContext("ShopId", shopId)
            """);
    }

    [Fact]
    public void ThenLogOrderCreated_With_ShopNameAndDivision()
    {
        Given((A<string>(), ASecond<string>()))
            .Then<ILogger>(_ => _.Information(
                It.Is<string>(s => s.Contains(A<string>()) && s.Contains(ASecond<string>()))));
        Specification.Is(
            """
            Given (a string, a second string)
            When _.PlaceOrder(a ShoppingCart)
            Then ILogger.Information(It.Is<string>(s => s.Contains(A<string>()) && s.
                  Contains(ASecond<string>())))
            """);
    }
}