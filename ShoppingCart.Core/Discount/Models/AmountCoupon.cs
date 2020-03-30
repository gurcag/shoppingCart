using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Common.Models;

namespace ShoppingCart.Core.Discount.Models
{
    public sealed class AmountCoupon : BaseCoupon, IAmountCoupon
    {
        public double DiscountAmount { get; }

        public AmountCoupon(double discountAmount, string title, bool hasMinCartAmountConstraint,
            bool hasMinCartItemQuantityConstraint, double minCartAmount, int minCartItemQuantity)
            : base(DiscountType.Amount, title, hasMinCartAmountConstraint,
                  hasMinCartItemQuantityConstraint, minCartAmount, minCartItemQuantity)
        {
            this.DiscountAmount = discountAmount;
        }
    }
}
