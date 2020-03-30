using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Common.Models;

namespace ShoppingCart.Core.Discount.Models
{
    public sealed class RateCampaign : BaseCampaign, IRateCampaign
    {
        public double DiscountRate { get; }

        public RateCampaign(ICategory category, string title, double discountRate, bool hasMinCartAmountConstraint,
            bool hasMinCartItemQuantityConstraint, double minCartAmount, int minCartItemQuantity)
            : base(category, title, DiscountType.Rate, hasMinCartAmountConstraint,
                  hasMinCartItemQuantityConstraint, minCartAmount, minCartItemQuantity)
        {
            this.DiscountRate = discountRate;
        }
    }
}
