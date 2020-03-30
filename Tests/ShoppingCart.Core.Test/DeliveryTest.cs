using Xunit;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Common.Models;
using ShoppingCart.Core.Delivery.Interfaces;
using ShoppingCart.Core.Delivery.Services;
using ShoppingCart.Core.Cart.Interfaces;

namespace ShoppingCart.Core.Test
{
    public class DeliveryTest
    {
        readonly IDeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(2, 0.5, 2.99);
        readonly ICategory category1, category2;
        readonly IProduct product1Category1, product2Category1, product3Category2, product4Category2;

        [Fact]
        public void EmptyCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            var deliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(deliveryCost, this.deliveryCostCalculator.FixedCost);
            Assert.Equal(deliveryCost, this.Calculate(0, 0));
        }

        [Fact]
        public void SingleItem_SingleQuantity_SingleCategory_OnCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 1);
            var deliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(deliveryCost, this.Calculate(1, 1));
        }

        [Fact]
        public void SingleItem_MultipleQuantity_SingleCategory_OnCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 4);
            var deliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(deliveryCost, this.Calculate(1, 1));
        }

        [Fact]
        public void MultipleItem_SingleQuantity_SingleCategory_OnCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 1);
            cart.Add(this.product2Category1, 1);
            var deliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(deliveryCost, this.Calculate(1, 2));
        }

        [Fact]
        public void MultipleItem_MultipleQuantity_SingleCategory_OnCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 3);
            cart.Add(this.product2Category1, 5);
            var deliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(deliveryCost, this.Calculate(1, 2));
        }

        [Fact]
        public void MultipleItem_MultipleQuantity_MultipleCategory_OnCart()
        {
            ICart cart = new ShoppingCart.Core.Cart.Services.Cart();
            cart.Add(this.product1Category1, 3);
            cart.Add(this.product4Category2, 5);
            var deliveryCost = this.deliveryCostCalculator.Calculate(cart);
            Assert.Equal(deliveryCost, this.Calculate(2, 2));
        }

        public DeliveryTest()
        {
            this.category1 = new Category("Category1");

            this.product1Category1 = new Product(category1, "product1Category1", 1.0);
            this.product2Category1 = new Product(category1, "product2Category1", 10.0);

            this.category2 = new Category("Category2");

            this.product3Category2 = new Product(category2, "product1Category2", 2.0);
            this.product4Category2 = new Product(category2, "product2Category2", 20.0);
        }

        double Calculate(int numberOfDeliveries, int numberOfProduct)
        {
            return (this.deliveryCostCalculator.CostPerDelivery * numberOfDeliveries) + (this.deliveryCostCalculator.CostPerProduct * numberOfProduct) + this.deliveryCostCalculator.FixedCost;
        }
    }
}
