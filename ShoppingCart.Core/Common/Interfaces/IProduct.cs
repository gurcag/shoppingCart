using System;

namespace ShoppingCart.Core.Common.Interfaces
{
    public interface IProduct : IEntity
    {
        double Price { get; set; }
        ICategory Category { get; set; }
    }
}
