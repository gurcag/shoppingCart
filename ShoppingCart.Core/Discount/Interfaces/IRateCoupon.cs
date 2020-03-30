namespace ShoppingCart.Core.Discount.Interfaces
{
    public interface IRateCoupon : ICoupon
    {
        double DiscountRate { get; }
    }
}
