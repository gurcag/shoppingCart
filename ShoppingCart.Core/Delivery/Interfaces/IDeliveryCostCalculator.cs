using ShoppingCart.Core.Cart.Interfaces;

namespace ShoppingCart.Core.Delivery.Interfaces
{
    public interface IDeliveryCostCalculator
    {
        double CostPerDelivery { get; }
        double CostPerProduct { get; }
        double FixedCost { get; }
        double Calculate(ICart cart);
    }
}
