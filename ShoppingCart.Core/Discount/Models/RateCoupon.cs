using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Common.Models;

namespace ShoppingCart.Core.Discount.Models
{
    public class RateCoupon : BaseCoupon, IRateCoupon
    {
        public double DiscountRate { get; }

        public RateCoupon(double discountRate, string title, bool hasMinCartAmountConstraint,
            bool hasMinCartItemQuantityConstraint, double minCartAmount, int minCartItemQuantity)
            : base(DiscountType.Rate, title, hasMinCartAmountConstraint,
                  hasMinCartItemQuantityConstraint, minCartAmount, minCartItemQuantity)
        {
            this.DiscountRate = discountRate;
        }
    }
}
