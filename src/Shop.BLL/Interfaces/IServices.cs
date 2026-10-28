using System.Collections.Generic;
using Shop.BLL.DTOs;
using Shop.Core.Entities;

namespace Shop.BLL.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllAvailableProducts();
        Product? GetProductById(int id);
        void AddProduct(Product product);
    }

    public interface IOrderService
    {
        Order CreateOrder(OrderCreateRequest request);
        List<Order> GetOrdersByUserId(int userId);
        bool CancelOrder(int orderId);
        decimal CalculateTotal(List<OrderItemRequest> items);
    }

    public interface IUserService
    {
        User RegisterUser(UserRegisterRequest request);
        User? GetUserById(int id);
        User? GetUserByEmail(string email);
    }
}
