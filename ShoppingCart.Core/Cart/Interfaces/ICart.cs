using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Discount.Interfaces;
using System.Collections.Generic;

namespace ShoppingCart.Core.Cart.Interfaces
{
    public interface ICart
    {
        ICollection<ICartItem> Items { get; }
        ICampaign AppliedCampaign { get; }
        ICoupon AppliedCoupon { get; }
        double TotalAmount { get; }
        double TotalCampaignDiscountAmount { get; }
        double TotalAmountAfterCampaignDiscount { get; }
        double TotalCouponDiscountAmount { get; }
        double TotalAmountAfterDiscounts { get; }
        double DeliveryCost { get; set; }
        void Add(IProduct product, int quantity);
        void ApplyDiscounts(IEnumerable<ICampaign> campaigns, IEnumerable<ICoupon> coupons);
        string Print();
    }
}
