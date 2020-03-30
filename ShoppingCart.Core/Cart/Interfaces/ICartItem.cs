using ShoppingCart.Core.Common.Interfaces;

namespace ShoppingCart.Core.Cart.Interfaces
{
    public interface ICartItem : IEntity
    {
        IProduct ActualProduct { get; }
        int Quantity { get; set; }
        double TotalPrice { get; set; }
        double PricePerProduct { get; set; }
    }
}
