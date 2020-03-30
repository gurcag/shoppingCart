namespace ShoppingCart.Core.Discount.Interfaces
{
    public interface IRateCampaign : ICampaign
    {
        double DiscountRate { get; }
    }
}
