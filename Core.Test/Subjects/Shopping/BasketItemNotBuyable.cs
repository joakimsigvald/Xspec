namespace Xspec.Test.Subjects.Shopping;

public class BasketItemNotBuyable : ApplicationException
{
    public BasketItemNotBuyable(string message)
        : base(message)
    {
    }
}