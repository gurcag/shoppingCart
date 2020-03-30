using System;
using System.Collections.Generic;
using System.Linq;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Cart.Interfaces;
using ShoppingCart.Core.Cart.Models;
using System.Text;
using System.Collections.ObjectModel;

namespace ShoppingCart.Core.Cart.Services
{
    public class Cart : ICart
    {
        public ICollection<ICartItem> Items { get; set; }
        public ICampaign AppliedCampaign { get; set; }
        public ICoupon AppliedCoupon { get; set; }
        public double TotalAmount { get; set; }
        public double TotalCampaignDiscountAmount { get; set; }
        public double TotalAmountAfterCampaignDiscount { get; set; }
        public double TotalCouponDiscountAmount { get; set; }
        public double TotalAmountAfterDiscounts { get; set; }
        public double DeliveryCost { get; set; }

        public Cart()
        {
            this.Items = new Collection<ICartItem>();
        }

        public void Add(IProduct product, int quantity)
        {
            bool isActualProductAlreadyExistInCart = this.Items.Any(x => x.ActualProduct.Id == product.Id);

            if (isActualProductAlreadyExistInCart)
            {
                var cartItem = this.Items.Single(x => x.ActualProduct.Id == product.Id);

                cartItem.Quantity += quantity;
                cartItem.TotalPrice = cartItem.PricePerProduct * cartItem.Quantity;
            }
            else
            {
                this.Items.Add(new CartItem(product, quantity));
            }

            this.UpdateAmounts();
        }

        public void ApplyDiscounts(IEnumerable<ICampaign> campaigns, IEnumerable<ICoupon> coupons)
        {
            this.ApplyCampaignDiscount(campaigns);

            this.ApplyCouponDiscount(coupons);
        }

        void UpdateAmounts()
        {
            this.TotalAmount = this.Items.Sum(x => x.Quantity * x.ActualProduct.Price);
            this.TotalAmountAfterCampaignDiscount = this.TotalAmount - this.TotalCampaignDiscountAmount;
            this.TotalAmountAfterDiscounts = this.TotalAmountAfterCampaignDiscount - this.TotalCouponDiscountAmount;
        }

        void ApplyDiscountOnCartItems(ICollection<ICartItem> cartItems, double discount)
        {
            double totalAmountCartItems = cartItems.Sum(x => x.PricePerProduct * x.Quantity);

            foreach (var cartItem in cartItems)
            {
                double discountPerProduct = discount * cartItem.PricePerProduct / totalAmountCartItems;

                cartItem.PricePerProduct = cartItem.PricePerProduct - discountPerProduct;
                cartItem.TotalPrice = cartItem.PricePerProduct * cartItem.Quantity;
            }
        }

        void ApplyCampaignDiscount(IEnumerable<ICampaign> campaigns)
        {
            if (campaigns != null && campaigns.Any())
            {
                ICampaign bestCampaign = default(ICampaign);

                double bestCampaignDiscountAmount = default(double);

                foreach (var campaign in campaigns)
                {
                    if (this.IsCampaignApplicable(campaign))
                    {
                        var totalDiscountAmountForCategory = this.GetCampaignDiscountAmount(campaign);

                        if (totalDiscountAmountForCategory > bestCampaignDiscountAmount)
                        {
                            bestCampaignDiscountAmount = totalDiscountAmountForCategory;
                            bestCampaign = campaign;
                        }
                    }
                }

                if (bestCampaign != default(ICampaign))
                {
                    this.AppliedCampaign = bestCampaign;

                    var bestCampaignCategoryItems = this.Items.Where(x => x.ActualProduct.Category.Id == bestCampaign.Category.Id).ToList();

                    this.ApplyDiscountOnCartItems(bestCampaignCategoryItems, bestCampaignDiscountAmount);
                }
                else
                {
                    this.AppliedCampaign = null;
                }

                this.TotalCampaignDiscountAmount = bestCampaignDiscountAmount;
                this.UpdateAmounts();

            }
            else
            {
                this.TotalAmountAfterCampaignDiscount = this.TotalAmount;
                this.UpdateAmounts();
            }
        }

        void ApplyCouponDiscount(IEnumerable<ICoupon> coupons)
        {
            if (coupons != null && coupons.Any())
            {
                ICoupon bestCoupon = default(ICoupon);

                double bestCouponDiscountAmount = default(double);

                foreach (var coupon in coupons)
                {
                    if (this.IsCouponApplicable(coupon))
                    {
                        var totalDiscountAmount = this.GetCouponDiscountAmount(coupon);

                        if (totalDiscountAmount > bestCouponDiscountAmount)
                        {
                            bestCouponDiscountAmount = totalDiscountAmount;
                            bestCoupon = coupon;
                        }
                    }
                }

                if (bestCoupon != default(ICoupon))
                {
                    this.AppliedCoupon = bestCoupon;
                    this.ApplyDiscountOnCartItems(this.Items, bestCouponDiscountAmount);
                }
                else
                {
                    this.AppliedCoupon = null;
                }

                this.TotalCouponDiscountAmount = bestCouponDiscountAmount;
                this.UpdateAmounts();
            }
            else
            {
                this.TotalAmountAfterDiscounts = this.TotalAmountAfterCampaignDiscount;
                this.UpdateAmounts();
            }
        }

        bool IsCampaignApplicable(ICampaign campaign)
        {
            return campaign.IsActive
                && (!campaign.HasMinCartAmountForCategoryConstraint || (this.GetTotalAmountForCategory(campaign.Category.Id) >= campaign.MinCartAmountForCategory))
                && (!campaign.HasMinCartItemQuantityForCategoryConstraint || (this.GetTotalQuantityForCategory(campaign) >= campaign.MinCartItemQuantityForCategory));
        }

        double GetTotalAmountForCategory(Guid categoryId)
        {
            return this.Items.Where(x => x.ActualProduct.Category.Id == categoryId)
                .Sum(x => x.TotalPrice);
        }

        int GetTotalQuantityForCategory(ICampaign campaign)
        {
            return this.Items.Where(x => x.ActualProduct.Category.Id == campaign.Category.Id)
                .Sum(x => x.Quantity);
        }

        double GetCampaignDiscountAmount(ICampaign campaign)
        {
            double discountAmount;

            if (campaign is IAmountCampaign)
            {
                var amountCampaign = campaign as IAmountCampaign;

                discountAmount = amountCampaign.DiscountAmount;
            }
            else if (campaign is IRateCampaign)
            {
                var rateCampaign = campaign as IRateCampaign;

                var totalPriceCampaignCategoryItems = this.Items
                    .Where(x => x.ActualProduct.Category.Id == campaign.Category.Id).ToList()
                    .Sum(x => x.ActualProduct.Price * x.Quantity);

                discountAmount = rateCampaign.DiscountRate * totalPriceCampaignCategoryItems;
            }
            else
                throw new NotImplementedException();

            return discountAmount;
        }

        bool IsCouponApplicable(ICoupon coupon)
        {
            return coupon.IsActive
                && (!coupon.HasMinCartAmountConstraint || (coupon.HasMinCartAmountConstraint && this.TotalAmountAfterCampaignDiscount >= coupon.MinCartAmount))
                && (!coupon.HasMinCartItemQuantityConstraint || (coupon.HasMinCartItemQuantityConstraint && GetTotalQuantity() >= coupon.MinCartItemQuantity));
        }

        double GetCouponDiscountAmount(ICoupon coupon)
        {
            double discountAmount;

            if (coupon is IAmountCoupon)
            {
                var amountCoupon = coupon as IAmountCoupon;

                discountAmount = amountCoupon.DiscountAmount;
            }
            else if (coupon is IRateCoupon)
            {
                var rateCoupon = coupon as IRateCoupon;

                discountAmount = rateCoupon.DiscountRate * this.TotalAmountAfterCampaignDiscount;
            }
            else
                throw new NotImplementedException();

            return discountAmount;
        }

        int GetTotalQuantity()
        {
            return this.Items.Sum(x => x.Quantity);
        }

        public string Print()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Cart Details").AppendLine()
                .AppendLine()
                .Append("Quantity Of Products : ").Append(this.Items.Sum(x => x.Quantity).ToString()).AppendLine()
                .Append("Count Distinct Products : ").Append(this.Items.GroupBy(x => x.ActualProduct.Id).Count().ToString()).AppendLine()
                .Append("Count Distinct Categories : ").Append(this.Items.GroupBy(x => x.ActualProduct.Category.Id).Count().ToString()).AppendLine()
                .Append("TotalAmount : ").Append(this.TotalAmount.ToString()).AppendLine()
                .Append("TotalCampaignDiscountAmount : ").Append(this.TotalCampaignDiscountAmount.ToString()).AppendLine()
                .Append("TotalAmountAfterCampaignDiscount : ").Append(this.TotalAmountAfterCampaignDiscount.ToString()).AppendLine()
                .Append("TotalCouponDiscountAmount : ").Append(this.TotalCouponDiscountAmount.ToString()).AppendLine()
                .Append("TotalAmountAfterDiscounts : ").Append(this.TotalAmountAfterDiscounts.ToString()).AppendLine()
                .AppendLine();

            if (this.AppliedCampaign != null)
            {
                stringBuilder.Append("AppliedCampaign Details : ").AppendLine()
                .AppendLine()
                .Append("Title : ").Append(this.AppliedCampaign.Title).AppendLine()
                .Append("Available Category : ").Append(this.AppliedCampaign.Category.Title).AppendLine()
                .Append("Has Min Cart Amount For Category Constraint : ").Append(this.AppliedCampaign.HasMinCartAmountForCategoryConstraint).AppendLine()
                .Append("Min Cart Amount For Category : ").Append(this.AppliedCampaign.MinCartAmountForCategory).AppendLine()
                .Append("Has Min Cart Item Quantity For Category Constraint : ").Append(this.AppliedCampaign.HasMinCartItemQuantityForCategoryConstraint).AppendLine()
                .Append("Min Cart Item Quantity For Category : ").Append(this.AppliedCampaign.MinCartItemQuantityForCategory).AppendLine();

                if (this.AppliedCampaign is IAmountCampaign)
                {
                    var amountCampaign = this.AppliedCampaign as IAmountCampaign;
                    stringBuilder.Append("DiscountAmount : ").Append(amountCampaign.DiscountAmount.ToString()).AppendLine().AppendLine();
                }
                else
                {
                    var rateCampaign = this.AppliedCampaign as IRateCampaign;
                    stringBuilder.Append("DiscountRate : ").Append(rateCampaign.DiscountRate.ToString()).AppendLine().AppendLine();
                }
            }

            if (this.AppliedCoupon != null)
            {
                stringBuilder.Append("AppliedCoupon Details : ").AppendLine()
                    .AppendLine()
                    .Append("Title : ").Append(this.AppliedCoupon.Title).AppendLine()
                    .Append("Has Min Cart Amount Constraint : ").Append(this.AppliedCoupon.HasMinCartAmountConstraint).AppendLine()
                    .Append("Min Cart Amount : ").Append(this.AppliedCoupon.MinCartAmount).AppendLine()
                    .Append("Has Min Cart Item Quantity Constraint : ").Append(this.AppliedCoupon.HasMinCartItemQuantityConstraint).AppendLine()
                    .Append("Min Cart Item Quantity : ").Append(this.AppliedCoupon.MinCartItemQuantity).AppendLine();

                if (this.AppliedCoupon is IAmountCoupon)
                {
                    var amountCoupon = this.AppliedCoupon as IAmountCoupon;
                    stringBuilder.Append("DiscountAmount : ").Append(amountCoupon.DiscountAmount.ToString()).AppendLine().AppendLine();
                }
                else
                {
                    var rateCoupon = this.AppliedCoupon as IRateCoupon;
                    stringBuilder.Append("DiscountRate : ").Append(rateCoupon.DiscountRate.ToString()).AppendLine().AppendLine();
                }
            }

            stringBuilder.Append("Product Details").AppendLine().AppendLine();

            foreach (var cartItem in this.Items.OrderBy(x => x.ActualProduct.Category.Id))
            {
                stringBuilder.Append("Product Name : ").Append(cartItem.ActualProduct.Title).AppendLine()
                    .Append("Category Name : ").Append(cartItem.ActualProduct.Category.Title).AppendLine()
                    .Append("Original Price : ").Append(cartItem.ActualProduct.Price.ToString()).AppendLine()
                    .Append("Quantity: ").Append(cartItem.Quantity.ToString()).AppendLine()
                    .Append("Price Per Product After Discounts : ").Append(cartItem.PricePerProduct.ToString()).AppendLine()
                    .Append("Total Price After Discounts: ").Append(cartItem.TotalPrice.ToString()).AppendLine()
                    .AppendLine().AppendLine();
            }

            stringBuilder.Append("Delivery Details").AppendLine().AppendLine()
                .Append("DeliveryCost : ").Append(this.DeliveryCost.ToString()).AppendLine();

            return stringBuilder.ToString();
        }
    }
}
