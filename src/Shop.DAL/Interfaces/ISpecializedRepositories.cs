using System.Collections.Generic;
using Shop.Core.Entities;

namespace Shop.DAL.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        List<Product> GetByCategoryId(int categoryId);
        Product? GetBySku(string sku);
        List<Product> GetInStock();
    }

    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetByUserId(int userId);
    }

    public interface IUserRepository : IRepository<User>
    {
        User? GetByEmail(string email);
    }
}
