using Xspec.Test.Subjects.Shopping;

namespace Xspec.Test.Subjects.Purchase;

public interface IBasketRepository
{
    Task<Basket> GetEditable(int basketId);
    Task<Basket> UpdateStatus(int basketId);
}