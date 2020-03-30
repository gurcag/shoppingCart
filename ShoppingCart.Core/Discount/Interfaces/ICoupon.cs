using ShoppingCart.Core.Common.Interfaces;

namespace ShoppingCart.Core.Discount.Interfaces
{
    public interface ICoupon : IEntity
    {
        bool HasMinCartAmountConstraint { get; }
        bool HasMinCartItemQuantityConstraint { get; }
        double MinCartAmount { get; }
        int MinCartItemQuantity { get; }
    }
}
