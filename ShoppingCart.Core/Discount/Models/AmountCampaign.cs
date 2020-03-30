using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Common.Models;

namespace ShoppingCart.Core.Discount.Models
{
    public sealed class AmountCampaign : BaseCampaign, IAmountCampaign
    {
        public double DiscountAmount { get; }

        public AmountCampaign(ICategory category, string title, double discountAmount, bool hasMinCartAmountConstraint,
            bool hasMinCartItemQuantityConstraint, double minCartAmount, int minCartItemQuantity)
            : base(category, title, DiscountType.Amount, hasMinCartAmountConstraint,
                  hasMinCartItemQuantityConstraint, minCartAmount, minCartItemQuantity)
        {
            this.DiscountAmount = discountAmount;
        }
    }
}
