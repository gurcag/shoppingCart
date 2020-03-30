namespace ShoppingCart.Core.Discount.Interfaces
{
    public interface IAmountCampaign : ICampaign
    {
        double DiscountAmount { get; }
    }
}
