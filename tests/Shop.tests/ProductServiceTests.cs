using Moq;
using Shop.BLL.Services;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;

namespace Shop.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> productRepository = new Mock<IProductRepository>();
        private readonly ProductService service;

        public ProductServiceTests()
        {
            service = new ProductService(productRepository.Object);
        }

        [Fact]
        public void GetAllAvailableProducts_ReturnsInStockList()
        {
            List<Product> expected = new List<Product> { new Product { Id = 1, Name = "A" } };
            productRepository.Setup(r => r.GetInStock()).Returns(expected);

            List<Product> actual = service.GetAllAvailableProducts();

            Assert.Single(actual);
            Assert.Equal("A", actual[0].Name);
        }

        [Fact]
        public void GetProductById_ReturnsProduct()
        {
            Product product = new Product { Id = 5, Name = "Phone" };
            productRepository.Setup(r => r.GetById(5)).Returns(product);

            Product? actual = service.GetProductById(5);

            Assert.NotNull(actual);
            Assert.Equal(5, actual!.Id);
        }

        [Fact]
        public void AddProduct_WhenNull_Throws()
        {
            Exception ex = Assert.Throws<Exception>(() => service.AddProduct(null!));
            Assert.Equal("Товар не может быть пустым.", ex.Message);
        }

        [Fact]
        public void AddProduct_WhenNegativePrice_Throws()
        {
            Product product = new Product { Name = "Bad", Price = -1 };
            Exception ex = Assert.Throws<Exception>(() => service.AddProduct(product));
            Assert.Equal("Цена товара не может быть отрицательной.", ex.Message);
        }

        [Fact]
        public void AddProduct_WhenValid_AddsToRepository()
        {
            Product product = new Product { Name = "OK", Price = 10 };
            service.AddProduct(product);
            productRepository.Verify(r => r.Add(product), Times.Once);
        }
    }
}
