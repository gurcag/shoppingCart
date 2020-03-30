using System;

namespace ShoppingCart.Core.Common.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; }
        DateTime CreatedDate { get; }
        DateTime ModifiedDate { get; set; }
        string Title { get; }
        bool IsActive { get; set; }
    }
}
