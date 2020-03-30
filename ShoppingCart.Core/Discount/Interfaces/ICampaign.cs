using ShoppingCart.Core.Common.Interfaces;

namespace ShoppingCart.Core.Discount.Interfaces
{
    public interface ICampaign : IEntity
    {
        ICategory Category { get; }
        bool HasMinCartAmountForCategoryConstraint { get; }
        bool HasMinCartItemQuantityForCategoryConstraint { get; }
        double MinCartAmountForCategory { get; }
        int MinCartItemQuantityForCategory { get; }
    }
}
