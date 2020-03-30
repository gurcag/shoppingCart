namespace ShoppingCart.Core.Common.Interfaces
{
    public interface ICategory : IEntity
    {
        ICategory ParentCategory { get; }
    }
}
