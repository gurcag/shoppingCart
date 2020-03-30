using System;
using ShoppingCart.Core.Common.Interfaces;

namespace ShoppingCart.Core.Common.Models
{
    public class Product : IProduct
    {
        public double Price { get; set; }
        public ICategory Category { get; set; }
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; }
        public bool IsActive { get; set; }

        public Product(ICategory category, string title, double price)
        {
            this.Id = Guid.NewGuid();
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
            this.Category = category;
            this.Title = title;
            this.Price = price;
        }
    }
}
