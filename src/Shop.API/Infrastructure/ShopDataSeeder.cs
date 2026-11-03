using Shop.Core.Entities;
using Shop.DAL.Interfaces;

namespace Shop.API.Infrastructure
{
    public static class ShopDataSeeder
    {
        public static void EnsureDemoData(IUserRepository userRepo, IProductRepository productRepo)
        {
            if (userRepo.GetByEmail("customer@shop.ru") == null)
            {
                userRepo.Add(new User
                {
                    Id = 1,
                    Email = "customer@shop.ru",
                    PasswordHash = "hashed_password",
                    FirstName = "Иван",
                    LastName = "Иванов",
                    Phone = "+7 (999) 000-11-22"
                });
            }

            if (productRepo.GetAll().Count == 0)
            {
                Category catLaptops = new Category { Id = 1, Name = "Ноутбуки" };
                Category catPhones = new Category { Id = 2, Name = "Смартфоны" };
                Category catAccessories = new Category { Id = 3, Name = "Аксессуары" };

                productRepo.Add(new Product { Id = 1, CategoryId = 1, Category = catLaptops, Name = "Ноутбук ASUS Vivobook 15", Sku = "ASUS-VB-15", Price = 65000, StockQuantity = 6, IsActive = true });
                productRepo.Add(new Product { Id = 2, CategoryId = 1, Category = catLaptops, Name = "Ноутбук Lenovo IdeaPad 3", Sku = "LNV-IP-3", Price = 52000, StockQuantity = 8, IsActive = true });
                productRepo.Add(new Product { Id = 3, CategoryId = 2, Category = catPhones, Name = "Смартфон Samsung Galaxy A54", Sku = "SM-A54-128", Price = 33000, StockQuantity = 12, IsActive = true });
                productRepo.Add(new Product { Id = 4, CategoryId = 2, Category = catPhones, Name = "Смартфон Xiaomi Redmi Note 13", Sku = "XMI-RN-13", Price = 24000, StockQuantity = 15, IsActive = true });
                productRepo.Add(new Product { Id = 5, CategoryId = 3, Category = catAccessories, Name = "Беспроводная мышь Logitech M185", Sku = "LOG-M185", Price = 1800, StockQuantity = 25, IsActive = true });
                productRepo.Add(new Product { Id = 6, CategoryId = 3, Category = catAccessories, Name = "Наушники JBL Tune 510BT", Sku = "JBL-T510", Price = 4500, StockQuantity = 10, IsActive = true });
            }
        }
    }
}
