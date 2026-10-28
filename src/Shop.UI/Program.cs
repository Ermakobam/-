using System;
using System.Collections.Generic;
using System.Text;
using Shop.BLL.DTOs;
using Shop.BLL.Interfaces;
using Shop.BLL.Services;
using Shop.Core.Entities;
using Shop.DAL.Repositories;

namespace Shop.UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            ProductRepository productRepo = new ProductRepository();
            OrderRepository orderRepo = new OrderRepository();
            UserRepository userRepo = new UserRepository();

            IProductService productService = new ProductService(productRepo);
            IUserService userService = new UserService(userRepo);
            IOrderService orderService = new OrderService(orderRepo, productRepo, userRepo);

            UserRegisterRequest userRequest = new UserRegisterRequest
            {
                Email = "ivanov@mail.ru",
                Password = "mypassword123",
                FirstName = "Иван",
                LastName = "Иванов",
                Phone = "+7 (999) 111-22-33"
            };

            User customer = userService.RegisterUser(userRequest);

            Category catLaptops = new Category { Id = 1, Name = "Ноутбуки", Description = "Портативные компьютеры" };
            Category catPhones = new Category { Id = 2, Name = "Смартфоны", Description = "Мобильные устройства" };

            productService.AddProduct(new Product
            {
                Id = 1,
                CategoryId = catLaptops.Id,
                Category = catLaptops,
                Name = "Ноутбук ASUS Vivobook 15",
                Sku = "ASUS-VB-15",
                Price = 65000,
                StockQuantity = 5,
                IsActive = true
            });

            productService.AddProduct(new Product
            {
                Id = 2,
                CategoryId = catLaptops.Id,
                Category = catLaptops,
                Name = "Ноутбук Lenovo IdeaPad 3",
                Sku = "LNV-IP-3",
                Price = 52000,
                StockQuantity = 8,
                IsActive = true
            });

            productService.AddProduct(new Product
            {
                Id = 3,
                CategoryId = catPhones.Id,
                Category = catPhones,
                Name = "Смартфон Samsung Galaxy A54",
                Sku = "SM-A54-128",
                Price = 33000,
                StockQuantity = 10,
                IsActive = true
            });

            List<Product> catalog = productService.GetAllAvailableProducts();
            foreach (Product p in catalog)
            {
                Console.WriteLine(p.Id + " | " + p.Sku + " | " + p.Name + " | " + p.Price + " руб. | Остаток: " + p.StockQuantity);
            }

            OrderCreateRequest newOrderRequest = new OrderCreateRequest
            {
                UserId = customer.Id,
                ShippingAddress = "г. Москва, ул. Ленина, д. 10, кв. 25",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductId = 1, Quantity = 1 },
                    new OrderItemRequest { ProductId = 3, Quantity = 2 }
                }
            };

            Order createdOrder = orderService.CreateOrder(newOrderRequest);
            Console.WriteLine("Заказ #" + createdOrder.Id + " успешно оформлен на сумму " + createdOrder.TotalAmount + " руб.");
        }
    }
}
