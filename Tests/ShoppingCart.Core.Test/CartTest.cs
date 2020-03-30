using System;
using Xunit;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Common.Models;
using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Discount.Models;
using ShoppingCart.Core.Delivery.Interfaces;
using ShoppingCart.Core.Delivery.Services;
using ShoppingCart.Core.Cart.Interfaces;
using ShoppingCart.Core.Cart.Models;
using ShoppingCart.Core.Cart.Services;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShoppingCart.Core.Test
{
    public class CartTest
    {
        readonly IDeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(2, 0.5, 2.99);
        readonly ICategory category1, category2;
        readonly IProduct product1Category1, product2Category1, product3Category2;


        [Fact]
        public void EmptyCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            Assert.Equal(default(double), cart.TotalAmount);
            Assert.Equal(default(double), cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(default(double), cart.TotalAmountAfterDiscounts);
            Assert.Equal(default(double), cart.TotalCampaignDiscountAmount);
            Assert.Equal(default(double), cart.TotalCouponDiscountAmount);
            Assert.Null(cart.AppliedCampaign);
            Assert.Null(cart.AppliedCoupon);
            Assert.Equal(0, cart.Items.Count);
            Assert.Equal(0, cart.Items.Sum(x => x.Quantity));
            cart.DeliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(this.deliveryCostCalculator.FixedCost, cart.DeliveryCost);
        }

        [Fact]
        public void EmptyCart_WithDiscounts()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();

            ICollection<ICampaign> campaigns = new Collection<ICampaign>()
            {
                new AmountCampaign(this.category1, "", 100.0, true, false, double.MaxValue, 0),
                new RateCampaign(this.category1, "", 0.5, false, true, 0, int.MaxValue)
            };
            ICollection<ICoupon> coupons = new Collection<ICoupon>() {
                new AmountCoupon(1000, "", true, false, double.MaxValue, 0),
                new RateCoupon(.5,"",false,true,0,int.MaxValue)
            };

            cart.ApplyDiscounts(campaigns, coupons);

            Assert.Equal(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterDiscounts);
            Assert.Equal(0, cart.TotalCampaignDiscountAmount);
            Assert.Equal(0, cart.TotalCouponDiscountAmount);
            Assert.Null(cart.AppliedCampaign);
            Assert.Null(cart.AppliedCoupon);
        }

        [Fact]
        public void SingleItem_WithoutDiscount()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 1);
            Assert.Equal(this.product1Category1.Price, cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct), cart.TotalAmount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterDiscounts);
            Assert.Equal(0, cart.TotalCampaignDiscountAmount);
            Assert.Equal(0, cart.TotalCouponDiscountAmount);
            Assert.Null(cart.AppliedCampaign);
            Assert.Null(cart.AppliedCoupon);
            Assert.Equal(1, cart.Items.Count);
            Assert.Equal(1, cart.Items.Sum(x => x.Quantity));
        }

        [Fact]
        public void MultipleItem_MultipleQuantity_MultipleCategory_WithoutDiscount()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 2);
            cart.Add(this.product2Category1, 5);
            cart.Add(this.product1Category1, 1);
            cart.Add(this.product3Category2, 10);
            Assert.Equal(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterDiscounts);
            Assert.Equal(0, cart.TotalCampaignDiscountAmount);
            Assert.Equal(0, cart.TotalCouponDiscountAmount);
            Assert.Null(cart.AppliedCampaign);
            Assert.Null(cart.AppliedCoupon);
        }

        [Fact]
        public void InapplicableCampaign_InapplicableCoupon()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 2);
            cart.Add(this.product2Category1, 5);
            cart.Add(this.product1Category1, 1);
            cart.Add(this.product3Category2, 10);

            ICollection<ICampaign> campaigns = new Collection<ICampaign>()
            {
                new AmountCampaign(this.category1, "", 100.0, true, false, double.MaxValue, 0),
                new RateCampaign(this.category1, "", 0.5, false, true, 0, int.MaxValue)
            };
            ICollection<ICoupon> coupons = new Collection<ICoupon>() {
                new AmountCoupon(1000, "", true, false, double.MaxValue, 0),
                new RateCoupon(.5,"",false,true,0,int.MaxValue)
            };

            cart.ApplyDiscounts(campaigns, coupons);

            Assert.Equal(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.TotalAmount, cart.TotalAmountAfterDiscounts);
            Assert.Equal(0, cart.TotalCampaignDiscountAmount);
            Assert.Equal(0, cart.TotalCouponDiscountAmount);
            Assert.Null(cart.AppliedCampaign);
            Assert.Null(cart.AppliedCoupon);
        }

        [Fact]
        public void ApplicableCampaign_InapplicableCoupon()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 1);
            cart.Add(this.product2Category1, 5);
            cart.Add(this.product3Category2, 10);

            IAmountCampaign applicableCampaign = new AmountCampaign(this.category1, "", 100.0, true, false, 1000, 0);

            ICollection<ICampaign> campaigns = new Collection<ICampaign>()
            {
                applicableCampaign,
                new RateCampaign(this.category1, "", 0.5, false, true, 0, int.MaxValue)
            };
            ICollection<ICoupon> coupons = new Collection<ICoupon>() {
                new AmountCoupon(1000, "", true, false, double.MaxValue, 0),
                new RateCoupon(.5,"",false,true,0,int.MaxValue)
            };

            cart.ApplyDiscounts(campaigns, coupons);

            Assert.NotEqual(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.NotEqual(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmountAfterDiscounts);
            Assert.Equal(cart.TotalAmount - cart.TotalCampaignDiscountAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.TotalAmount - cart.TotalCampaignDiscountAmount, cart.TotalAmountAfterDiscounts);
            Assert.Equal(applicableCampaign.DiscountAmount, cart.TotalCampaignDiscountAmount);
            Assert.Equal(0, cart.TotalCouponDiscountAmount);
            Assert.Equal(applicableCampaign, cart.AppliedCampaign);
            Assert.Null(cart.AppliedCoupon);
        }

        [Fact]
        public void ApplicableCoupon_InapplicableCampaign()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 1);
            cart.Add(this.product2Category1, 5);
            cart.Add(this.product3Category2, 10);

            ICollection<ICampaign> campaigns = new Collection<ICampaign>()
            {
                new AmountCampaign(this.category1, "", 100.0, true, false, double.MaxValue, 0),
                new RateCampaign(this.category1, "", 0.5, false, true, 0, int.MaxValue)
            };

            IAmountCoupon applicableCoupon = new AmountCoupon(100, "", true, true, 1000, 1);

            ICollection<ICoupon> coupons = new Collection<ICoupon>() {
                applicableCoupon,
                new AmountCoupon(1000, "", true, false, double.MaxValue, 0),
                new RateCoupon(.5,"",false,true,0,int.MaxValue),
            };

            cart.ApplyDiscounts(campaigns, coupons);

            Assert.NotEqual(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.NotEqual(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmountAfterDiscounts);
            Assert.Equal(cart.TotalAmount - cart.TotalCampaignDiscountAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(0, cart.TotalCampaignDiscountAmount);
            Assert.Equal(applicableCoupon.DiscountAmount, cart.TotalCouponDiscountAmount);
            Assert.Null(cart.AppliedCampaign);
            Assert.Equal(applicableCoupon, cart.AppliedCoupon);
        }

        [Fact]
        public void ApplicableMultipleCampaigns_ApplicableMultipleCoupons()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 1);
            cart.Add(this.product2Category1, 5);
            cart.Add(this.product3Category2, 10);

            ICollection<ICampaign> applicableCampaigns = new Collection<ICampaign>()
            {
                new AmountCampaign(this.category1, "", 100.0, true, false, 1000, 0),
                new RateCampaign(this.category1, "", 0.5, true, true, 1, 1)
            };

            ICollection<ICoupon> applicableCoupons = new Collection<ICoupon>() {
            new AmountCoupon(100, "", true, true, 1000, 1),
            new AmountCoupon(1000, "", true, true, 1000, 1),
            new RateCoupon(.5, "", false, true, 0, int.MaxValue),
            };

            cart.ApplyDiscounts(applicableCampaigns, applicableCoupons);

            Assert.NotEqual(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmount);
            Assert.NotEqual(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmount);
            Assert.NotEqual(cart.Items.Sum(x => x.TotalPrice), cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.Items.Sum(x => x.PricePerProduct * x.Quantity), cart.TotalAmountAfterDiscounts);
            Assert.Equal(cart.TotalAmount - cart.TotalCampaignDiscountAmount, cart.TotalAmountAfterCampaignDiscount);
            Assert.Equal(cart.TotalAmountAfterCampaignDiscount - cart.TotalCouponDiscountAmount, cart.TotalAmountAfterDiscounts);
            Assert.Equal(10000, cart.TotalCampaignDiscountAmount);
            Assert.Equal(1000, cart.TotalCouponDiscountAmount);
            Assert.True(cart.AppliedCampaign is IRateCampaign);
            Assert.True(cart.AppliedCoupon is IAmountCoupon);
        }


        public CartTest()
        {
            this.category1 = new Category("Category1");
            this.product1Category1 = new Product(category1, "product1Category1", 10000.0);
            this.product2Category1 = new Product(category1, "product2Category1", 2000.0);

            this.category2 = new Category("Category2");
            this.product3Category2 = new Product(category2, "product1Category2", 100.0);
        }
    }
}
