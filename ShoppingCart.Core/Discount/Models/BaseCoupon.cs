using ShoppingCart.Core.Common.Models;
using ShoppingCart.Core.Discount.Interfaces;
using System;

namespace ShoppingCart.Core.Discount.Models
{
    public abstract class BaseCoupon : ICoupon
    {
        public DiscountType DiscountType { get; }
        public bool HasMinCartAmountConstraint { get; }
        public bool HasMinCartItemQuantityConstraint { get; }
        public double MinCartAmount { get; }
        public int MinCartItemQuantity { get; }
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; }
        public bool IsActive { get; set; }

        protected BaseCoupon(DiscountType discountType, string title, bool hasMinCartAmountConstraint, bool hasMinCartItemQuantityConstraint,
            double minCartAmount, int minCartItemQuantity)
        {
            this.Id = Guid.NewGuid();
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
            this.Title = title;
            this.DiscountType = discountType;
            this.HasMinCartAmountConstraint = hasMinCartAmountConstraint;
            this.HasMinCartItemQuantityConstraint = hasMinCartItemQuantityConstraint;
            this.MinCartAmount = minCartAmount;
            this.MinCartItemQuantity = minCartItemQuantity;
        }
    }
}
