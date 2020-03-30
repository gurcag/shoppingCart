namespace ShoppingCart.Core.Discount.Interfaces
{
    public interface IAmountCoupon : ICoupon
    {
        double DiscountAmount { get; }
    }
}
