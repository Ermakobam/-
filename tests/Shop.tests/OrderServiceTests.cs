using Moq;
using Shop.BLL.DTOs;
using Shop.BLL.Services;
using Shop.Core.Entities;
using Shop.Core.Enums;
using Shop.DAL.Interfaces;

namespace Shop.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> orderRepository = new Mock<IOrderRepository>();
        private readonly Mock<IProductRepository> productRepository = new Mock<IProductRepository>();
        private readonly Mock<IUserRepository> userRepository = new Mock<IUserRepository>();
        private readonly OrderService service;

        public OrderServiceTests()
        {
            service = new OrderService(orderRepository.Object, productRepository.Object, userRepository.Object);
        }

        private static Product ActiveProduct(int id = 1, decimal price = 100, int stock = 10)
        {
            return new Product { Id = id, Name = "Item" + id, Price = price, StockQuantity = stock, IsActive = true };
        }

        [Fact]
        public void CalculateTotal_WhenItemsNullOrEmpty_ReturnsZero()
        {
            Assert.Equal(0, service.CalculateTotal(null!));
            Assert.Equal(0, service.CalculateTotal(new List<OrderItemRequest>()));
        }

        [Fact]
        public void CalculateTotal_SkipsMissingProducts_AndSumsFound()
        {
            productRepository.Setup(r => r.GetById(1)).Returns(ActiveProduct(1, 50));
            productRepository.Setup(r => r.GetById(2)).Returns((Product?)null);

            decimal total = service.CalculateTotal(new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = 1, Quantity = 2 },
                new OrderItemRequest { ProductId = 2, Quantity = 5 }
            });

            Assert.Equal(100, total);
        }

        [Fact]
        public void CreateOrder_WhenRequestNull_Throws()
        {
            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(null!));
            Assert.Equal("Запрос на создание заказа не может быть пустым.", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenItemsNull_Throws()
        {
            OrderCreateRequest request = new OrderCreateRequest { UserId = 1, Items = null! };
            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Equal("Заказ должен содержать хотя бы один товар.", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenItemsEmpty_Throws()
        {
            OrderCreateRequest request = new OrderCreateRequest { UserId = 1, Items = new List<OrderItemRequest>() };
            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Equal("Заказ должен содержать хотя бы один товар.", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenUserMissing_Throws()
        {
            userRepository.Setup(r => r.GetById(9)).Returns((User?)null);
            OrderCreateRequest request = new OrderCreateRequest
            {
                UserId = 9,
                Items = new List<OrderItemRequest> { new OrderItemRequest { ProductId = 1, Quantity = 1 } }
            };

            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Contains("не найден", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenQuantityNotPositive_Throws()
        {
            userRepository.Setup(r => r.GetById(1)).Returns(new User { Id = 1 });
            OrderCreateRequest request = new OrderCreateRequest
            {
                UserId = 1,
                Items = new List<OrderItemRequest> { new OrderItemRequest { ProductId = 1, Quantity = 0 } }
            };

            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Equal("Количество товара должно быть больше нуля.", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenProductMissing_Throws()
        {
            userRepository.Setup(r => r.GetById(1)).Returns(new User { Id = 1 });
            productRepository.Setup(r => r.GetById(8)).Returns((Product?)null);
            OrderCreateRequest request = new OrderCreateRequest
            {
                UserId = 1,
                Items = new List<OrderItemRequest> { new OrderItemRequest { ProductId = 8, Quantity = 1 } }
            };

            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Contains("не найден или недоступен", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenProductInactive_Throws()
        {
            userRepository.Setup(r => r.GetById(1)).Returns(new User { Id = 1 });
            productRepository.Setup(r => r.GetById(1)).Returns(new Product { Id = 1, IsActive = false, StockQuantity = 5 });
            OrderCreateRequest request = new OrderCreateRequest
            {
                UserId = 1,
                Items = new List<OrderItemRequest> { new OrderItemRequest { ProductId = 1, Quantity = 1 } }
            };

            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Contains("не найден или недоступен", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenStockInsufficient_Throws()
        {
            userRepository.Setup(r => r.GetById(1)).Returns(new User { Id = 1 });
            productRepository.Setup(r => r.GetById(1)).Returns(ActiveProduct(stock: 1));
            OrderCreateRequest request = new OrderCreateRequest
            {
                UserId = 1,
                Items = new List<OrderItemRequest> { new OrderItemRequest { ProductId = 1, Quantity = 3 } }
            };

            Exception ex = Assert.Throws<Exception>(() => service.CreateOrder(request));
            Assert.Contains("Недостаточно товара", ex.Message);
        }

        [Fact]
        public void CreateOrder_WhenValid_DecrementsStockAndSavesOrder()
        {
            User user = new User { Id = 2, Email = "u@mail.ru" };
            Product product = ActiveProduct(1, 40, 5);
            userRepository.Setup(r => r.GetById(2)).Returns(user);
            productRepository.Setup(r => r.GetById(1)).Returns(product);

            Order created = service.CreateOrder(new OrderCreateRequest
            {
                UserId = 2,
                ShippingAddress = "Москва",
                Items = new List<OrderItemRequest> { new OrderItemRequest { ProductId = 1, Quantity = 2 } }
            });

            Assert.Equal(2, created.UserId);
            Assert.Equal(80, created.TotalAmount);
            Assert.Equal(OrderStatus.New, created.Status);
            Assert.Equal("Москва", created.ShippingAddress);
            Assert.Single(created.Items);
            Assert.Equal(3, product.StockQuantity);
            orderRepository.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            productRepository.Verify(r => r.Update(product), Times.Once);
        }

        [Fact]
        public void GetOrdersByUserId_DelegatesToRepository()
        {
            List<Order> orders = new List<Order> { new Order { Id = 1, UserId = 4 } };
            orderRepository.Setup(r => r.GetByUserId(4)).Returns(orders);
            Assert.Single(service.GetOrdersByUserId(4));
        }

        [Fact]
        public void CancelOrder_WhenMissing_ReturnsFalse()
        {
            orderRepository.Setup(r => r.GetById(1)).Returns((Order?)null);
            Assert.False(service.CancelOrder(1));
        }

        [Theory]
        [InlineData(OrderStatus.Cancelled)]
        [InlineData(OrderStatus.Delivered)]
        public void CancelOrder_WhenTerminalStatus_ReturnsFalse(OrderStatus status)
        {
            orderRepository.Setup(r => r.GetById(1)).Returns(new Order { Id = 1, Status = status });
            Assert.False(service.CancelOrder(1));
        }

        [Fact]
        public void CancelOrder_WhenUnknownStatus_ReturnsFalse()
        {
            orderRepository.Setup(r => r.GetById(1)).Returns(new Order { Id = 1, Status = (OrderStatus)99 });
            Assert.False(service.CancelOrder(1));
        }

        [Theory]
        [InlineData(OrderStatus.New)]
        [InlineData(OrderStatus.Paid)]
        [InlineData(OrderStatus.Processing)]
        [InlineData(OrderStatus.Shipped)]
        public void CancelOrder_WhenCancellable_RestoresStock(OrderStatus status)
        {
            Product product = ActiveProduct(1, 10, 1);
            Order order = new Order
            {
                Id = 7,
                Status = status,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 },
                    new OrderItem { ProductId = 99, Quantity = 1 }
                }
            };
            orderRepository.Setup(r => r.GetById(7)).Returns(order);
            productRepository.Setup(r => r.GetById(1)).Returns(product);
            productRepository.Setup(r => r.GetById(99)).Returns((Product?)null);

            Assert.True(service.CancelOrder(7));
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(3, product.StockQuantity);
            orderRepository.Verify(r => r.Update(order), Times.Once);
        }

        [Fact]
        public void CancelOrder_WhenItemsNull_StillCancels()
        {
            Order order = new Order { Id = 3, Status = OrderStatus.New, Items = null! };
            orderRepository.Setup(r => r.GetById(3)).Returns(order);

            Assert.True(service.CancelOrder(3));
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
    }
}
