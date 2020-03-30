using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Cart.Interfaces;
using System;

namespace ShoppingCart.Core.Cart.Models
{
    public class CartItem : ICartItem
    {
        public IProduct ActualProduct { get; }

        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double PricePerProduct { get; set; }

        public Guid Id { get; }

        public DateTime CreatedDate { get; }

        public DateTime ModifiedDate { get; set; }
        public string Title { get; }
        public bool IsActive { get; set; }

        public CartItem(IProduct actualProduct, int quantity)
        {
            this.Id = Guid.NewGuid();
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
            this.ActualProduct = actualProduct;
            this.Quantity = quantity;
            this.PricePerProduct = this.ActualProduct.Price;
            this.TotalPrice = this.PricePerProduct * this.Quantity;
        }
    }
}
