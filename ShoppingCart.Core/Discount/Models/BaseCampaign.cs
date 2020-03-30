using System;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Common.Models;
using ShoppingCart.Core.Discount.Interfaces;

namespace ShoppingCart.Core.Discount.Models
{
    public abstract class BaseCampaign : ICampaign
    {
        public DiscountType DiscountType { get; }
        public ICategory Category { get; }
        public bool HasMinCartAmountForCategoryConstraint { get; }
        public bool HasMinCartItemQuantityForCategoryConstraint { get; }
        public double MinCartAmountForCategory { get; }
        public int MinCartItemQuantityForCategory { get; }
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; }
        public bool IsActive { get; set; }

        protected BaseCampaign(ICategory category, string title, DiscountType discountType, bool hasMinCartAmountForCategoryConstraint
            , bool hasMinCartItemQuantityForCategoryConstraint, double minCartAmount, int minCartItemQuantity)
        {
            this.Id = Guid.NewGuid();
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
            this.Category = category;
            this.Title = title;
            this.DiscountType = discountType;
            this.HasMinCartAmountForCategoryConstraint = hasMinCartAmountForCategoryConstraint;
            this.HasMinCartItemQuantityForCategoryConstraint = hasMinCartItemQuantityForCategoryConstraint;
            this.MinCartAmountForCategory = minCartAmount;
            this.MinCartItemQuantityForCategory = minCartItemQuantity;
        }
    }
}
