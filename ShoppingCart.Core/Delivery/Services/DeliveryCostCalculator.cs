using System;
using System.Linq;
using ShoppingCart.Core.Cart.Interfaces;
using ShoppingCart.Core.Delivery.Interfaces;

namespace ShoppingCart.Core.Delivery.Services
{
    public class DeliveryCostCalculator : IDeliveryCostCalculator
    {
        public double CostPerDelivery { get; }
        public double CostPerProduct { get; }
        public double FixedCost { get; }

        public DeliveryCostCalculator(double costPerDelivery, double costPerProduct, double fixedCost)
        {
            this.CostPerDelivery = costPerDelivery;
            this.CostPerProduct = costPerProduct;
            this.FixedCost = fixedCost;
        }

        public double Calculate(ICart cart)
        {
            int numberOfDeliveries = default(int);
            int numberOfProduct = default(int);

            if (cart != null)
            {
                numberOfDeliveries = cart.Items.GroupBy(x => x.ActualProduct.Category.Id).Count();

                numberOfProduct = cart.Items.Count;
            }
            else
            {
                throw new ArgumentNullException();
            }

            return (this.CostPerDelivery * numberOfDeliveries) + (this.CostPerProduct * numberOfProduct) + this.FixedCost;
        }
    }
}
