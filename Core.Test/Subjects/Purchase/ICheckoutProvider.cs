using Xspec.Test.Subjects.Order;

namespace Xspec.Test.Subjects.Purchase;

public interface ICheckoutProvider
{
    Task<Checkout> GetExistingCheckout(int basketId);
}