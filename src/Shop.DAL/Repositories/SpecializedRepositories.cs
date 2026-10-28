using System.Collections.Generic;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;

namespace Shop.DAL.Repositories
{
    public class ProductRepository : InMemoryRepository<Product>, IProductRepository
    {
        public List<Product> GetByCategoryId(int categoryId)
        {
            List<Product> result = new List<Product>();
            foreach (Product product in items)
            {
                if (product.CategoryId == categoryId && product.IsActive)
                {
                    result.Add(product);
                }
            }
            return result;
        }

        public Product? GetBySku(string sku)
        {
            foreach (Product product in items)
            {
                if (product.Sku.ToLower() == sku.ToLower())
                {
                    return product;
                }
            }
            return null;
        }

        public List<Product> GetInStock()
        {
            List<Product> result = new List<Product>();
            foreach (Product product in items)
            {
                if (product.IsActive && product.StockQuantity > 0)
                {
                    result.Add(product);
                }
            }
            return result;
        }
    }

    public class OrderRepository : InMemoryRepository<Order>, IOrderRepository
    {
        public List<Order> GetByUserId(int userId)
        {
            List<Order> result = new List<Order>();
            foreach (Order order in items)
            {
                if (order.UserId == userId)
                {
                    result.Add(order);
                }
            }
            return result;
        }
    }

    public class UserRepository : InMemoryRepository<User>, IUserRepository
    {
        public User? GetByEmail(string email)
        {
            foreach (User user in items)
            {
                if (user.Email.ToLower() == email.ToLower())
                {
                    return user;
                }
            }
            return null;
        }
    }
}
