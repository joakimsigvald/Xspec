using Xspec.Test.Subjects.Shopping;

namespace Xspec.Test.Subjects.Order;

public class Checkout
{
    public Basket Basket { get; set; } = null!;
    public bool IsOpen { get; set; }
}