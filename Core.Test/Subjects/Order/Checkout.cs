using Xspec.Test.Subjects.Shopping;

namespace Xspec.Test.Subjects.Order;

public class Checkout
{
    public Basket Basket { get; set; }
    public bool IsOpen { get; set; }
}