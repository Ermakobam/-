using System;
using System.Collections.Generic;
using System.Text;
using Shop.BLL.DTOs;
using Shop.BLL.Interfaces;
using Shop.Core.Entities;
using Shop.Core.Enums;
using Shop.DAL.Interfaces;

namespace Shop.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public List<Product> GetAllAvailableProducts()
        {
            return productRepository.GetInStock();
        }

        public Product? GetProductById(int id)
        {
            return productRepository.GetById(id);
        }

        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw new Exception("Товар не может быть пустым.");
            }

            if (product.Price < 0)
            {
                throw new Exception("Цена товара не может быть отрицательной.");
            }

            productRepository.Add(product);
        }
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly IUserRepository userRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.userRepository = userRepository;
        }

        public decimal CalculateTotal(List<OrderItemRequest> items)
        {
            decimal total = 0;
            foreach (OrderItemRequest item in items)
            {
                Product? product = productRepository.GetById(item.ProductId);
                if (product != null)
                {
                    total += product.Price * item.Quantity;
                }
            }
            return total;
        }

        public Order CreateOrder(OrderCreateRequest request)
        {
            if (request == null)
            {
                throw new Exception("Запрос на создание заказа не может быть пустым.");
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                throw new Exception("Заказ должен содержать хотя бы один товар.");
            }

            User? user = userRepository.GetById(request.UserId);
            if (user == null)
            {
                throw new Exception("Пользователь с Id=" + request.UserId + " не найден.");
            }

            List<OrderItem> orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (OrderItemRequest itemReq in request.Items)
            {
                if (itemReq.Quantity <= 0)
                {
                    throw new Exception("Количество товара должно быть больше нуля.");
                }

                Product? product = productRepository.GetById(itemReq.ProductId);
                if (product == null || !product.IsActive)
                {
                    throw new Exception("Товар с Id=" + itemReq.ProductId + " не найден или недоступен.");
                }

                if (product.StockQuantity < itemReq.Quantity)
                {
                    throw new Exception("Недостаточно товара '" + product.Name + "' на складе. Запрошено: " + itemReq.Quantity + ", в наличии: " + product.StockQuantity + ".");
                }

                product.StockQuantity -= itemReq.Quantity;
                productRepository.Update(product);

                OrderItem orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = itemReq.Quantity,
                    UnitPrice = product.Price
                };

                orderItems.Add(orderItem);
                totalAmount += product.Price * itemReq.Quantity;
            }

            Order order = new Order
            {
                UserId = user.Id,
                User = user,
                OrderDate = DateTime.Now,
                Status = OrderStatus.New,
                ShippingAddress = request.ShippingAddress,
                TotalAmount = totalAmount,
                Items = orderItems
            };

            orderRepository.Add(order);
            return order;
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            return orderRepository.GetByUserId(userId);
        }

        public bool CancelOrder(int orderId)
        {
            Order? order = orderRepository.GetById(orderId);
            if (order == null || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
            {
                return false;
            }

            foreach (OrderItem item in order.Items)
            {
                Product? product = productRepository.GetById(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    productRepository.Update(product);
                }
            }

            order.Status = OrderStatus.Cancelled;
            orderRepository.Update(order);
            return true;
        }
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User RegisterUser(UserRegisterRequest request)
        {
            if (request == null)
            {
                throw new Exception("Данные пользователя не могут быть пустыми.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new Exception("Email обязателен для заполнения.");
            }

            User? existingUser = userRepository.GetByEmail(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Пользователь с email " + request.Email + " уже зарегистрирован.");
            }

            User newUser = new User
            {
                Email = request.Email,
                PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password)),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                CreatedAt = DateTime.Now
            };

            userRepository.Add(newUser);
            return newUser;
        }

        public User? GetUserById(int id)
        {
            return userRepository.GetById(id);
        }

        public User? GetUserByEmail(string email)
        {
            return userRepository.GetByEmail(email);
        }
    }
}
