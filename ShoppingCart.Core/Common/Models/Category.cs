using System;
using ShoppingCart.Core.Common.Interfaces;

namespace ShoppingCart.Core.Common.Models
{
    public class Category : ICategory
    {
        public ICategory ParentCategory { get; }

        public Guid Id { get; }

        public DateTime CreatedDate { get; }

        public DateTime ModifiedDate { get; set; }
        public string Title { get; }
        public bool IsActive { get; set; }

        public Category(string title, ICategory parentCategory = null)
        {
            this.Id = Guid.NewGuid();
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
            this.ParentCategory = parentCategory;
            this.Title = title;
        }
    }
}
