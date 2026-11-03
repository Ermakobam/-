using Shop.Core.Entities;
using Shop.DAL.Repositories;

namespace Shop.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public void InMemoryRepository_CrudAndIdGeneration()
        {
            InMemoryRepository<User> repo = new InMemoryRepository<User>();

            User first = new User { Email = "a@mail.ru" };
            repo.Add(first);
            Assert.Equal(1, first.Id);

            User second = new User { Id = 10, Email = "b@mail.ru" };
            repo.Add(second);
            Assert.Equal(10, second.Id);

            User third = new User { Email = "c@mail.ru" };
            repo.Add(third);
            Assert.Equal(11, third.Id);

            Assert.Equal(3, repo.GetAll().Count);
            Assert.Equal("a@mail.ru", repo.GetById(1)!.Email);
            Assert.Null(repo.GetById(999));

            first.FirstName = "Ann";
            repo.Update(first);
            Assert.Equal("Ann", repo.GetById(1)!.FirstName);

            repo.Update(new User { Id = 123, Email = "missing" });
            repo.Delete(999);
            repo.Delete(1);
            Assert.Null(repo.GetById(1));
            Assert.Equal(2, repo.GetAll().Count);
        }

        [Fact]
        public void ProductRepository_FiltersByCategorySkuAndStock()
        {
            ProductRepository repo = new ProductRepository();
            repo.Add(new Product { CategoryId = 1, Sku = "AAA", IsActive = true, StockQuantity = 2, Name = "A" });
            repo.Add(new Product { CategoryId = 1, Sku = "BBB", IsActive = false, StockQuantity = 5, Name = "B" });
            repo.Add(new Product { CategoryId = 2, Sku = "CCC", IsActive = true, StockQuantity = 0, Name = "C" });
            repo.Add(new Product { CategoryId = 2, Sku = "DDD", IsActive = true, StockQuantity = 4, Name = "D" });

            Assert.Single(repo.GetByCategoryId(1));
            Assert.Equal("A", repo.GetByCategoryId(1)[0].Name);
            Assert.Empty(repo.GetByCategoryId(9));

            Assert.Null(repo.GetBySku(null!));
            Assert.Null(repo.GetBySku(""));
            Assert.Equal("AAA", repo.GetBySku("aaa")!.Sku);
            Assert.Null(repo.GetBySku("zzz"));

            List<Product> inStock = repo.GetInStock();
            Assert.Equal(2, inStock.Count);
            Assert.Contains(inStock, p => p.Sku == "AAA");
            Assert.Contains(inStock, p => p.Sku == "DDD");
        }

        [Fact]
        public void OrderRepository_GetByUserId()
        {
            OrderRepository repo = new OrderRepository();
            repo.Add(new Order { UserId = 1 });
            repo.Add(new Order { UserId = 2 });
            repo.Add(new Order { UserId = 1 });

            Assert.Equal(2, repo.GetByUserId(1).Count);
            Assert.Empty(repo.GetByUserId(8));
        }

        [Fact]
        public void UserRepository_GetByEmail()
        {
            UserRepository repo = new UserRepository();
            repo.Add(new User { Email = "User@Mail.Ru" });

            Assert.Null(repo.GetByEmail(null!));
            Assert.Null(repo.GetByEmail(""));
            Assert.NotNull(repo.GetByEmail("user@mail.ru"));
            Assert.Null(repo.GetByEmail("none@mail.ru"));
        }
    }
}
