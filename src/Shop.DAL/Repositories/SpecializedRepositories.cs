using System;
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
            if (string.IsNullOrEmpty(sku))
            {
                return null;
            }

            foreach (Product product in items)
            {
                if (string.Equals(product.Sku, sku, StringComparison.OrdinalIgnoreCase))
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
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            foreach (User user in items)
            {
                if (string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
