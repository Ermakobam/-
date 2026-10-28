using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Lab2.Part2_Repository
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    public class Product : BaseEntity
    {
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public override string ToString()
        {
            return "ID=" + Id + " | " + Name + " | Категория: " + Category + " | Цена: " + Price + " руб. | Остаток: " + StockQuantity + " шт.";
        }
    }

    public interface IRepository<T> where T : BaseEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }

    public interface IProductRepository : IRepository<Product>
    {
        List<Product> GetByCategory(string category);
        List<Product> GetByPriceRange(decimal min, decimal max);
    }

    public class JsonRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly string filePath;
        protected List<T> items;
        private int nextId;

        public JsonRepository(string fileName)
        {
            filePath = fileName;
            items = Load();
            nextId = 1;
            foreach (T item in items)
            {
                if (item.Id >= nextId)
                {
                    nextId = item.Id + 1;
                }
            }
        }

        private List<T> Load()
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        protected void Save()
        {
            string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public virtual T? GetById(int id)
        {
            foreach (T item in items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return new List<T>(items);
        }

        public virtual void Add(T entity)
        {
            if (entity.Id <= 0)
            {
                entity.Id = nextId;
                nextId++;
            }
            items.Add(entity);
            Save();
        }

        public virtual void Update(T entity)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == entity.Id)
                {
                    items[i] = entity;
                    Save();
                    return;
                }
            }
        }

        public virtual void Delete(int id)
        {
            T? item = GetById(id);
            if (item != null)
            {
                items.Remove(item);
                Save();
            }
        }
    }

    public class ProductRepository : JsonRepository<Product>, IProductRepository
    {
        public ProductRepository(string fileName) : base(fileName)
        {
        }

        public List<Product> GetByCategory(string category)
        {
            List<Product> result = new List<Product>();
            foreach (Product p in items)
            {
                if (p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public List<Product> GetByPriceRange(decimal min, decimal max)
        {
            List<Product> result = new List<Product>();
            foreach (Product p in items)
            {
                if (p.Price >= min && p.Price <= max)
                {
                    result.Add(p);
                }
            }
            return result;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("ЧАСТЬ 2\n");

            string dbFile = "repository_products.json";
            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }

            IProductRepository repository = new ProductRepository(dbFile);

            Console.WriteLine("[Операция 1] Добавление записей через Add():");
            repository.Add(new Product { Name = "Ноутбук ASUS VivoBook", Category = "Ноутбуки", Price = 65000, StockQuantity = 5 });
            repository.Add(new Product { Name = "Ноутбук Lenovo ThinkPad", Category = "Ноутбуки", Price = 89000, StockQuantity = 3 });
            repository.Add(new Product { Name = "Смартфон Samsung Galaxy", Category = "Смартфоны", Price = 34000, StockQuantity = 10 });
            repository.Add(new Product { Name = "Наушники Sony WH-1000", Category = "Аудио", Price = 28000, StockQuantity = 7 });
            Console.WriteLine("-> Записи успешно добавлены и сохранены в файл: " + dbFile + "\n");

            Console.WriteLine("[Операция 2] Чтение всех записей через GetAll():");
            foreach (Product p in repository.GetAll())
            {
                Console.WriteLine("  " + p);
            }
            Console.WriteLine();

            Console.WriteLine("[Операция 3] Поиск по ID через GetById(2):");
            Product? found = repository.GetById(2);
            Console.WriteLine("-> Найден: " + found + "\n");

            Console.WriteLine("[Операция 4] Обновление цены через Update():");
            if (found != null)
            {
                found.Price = 82000;
                found.StockQuantity = 4;
                repository.Update(found);
                Console.WriteLine("-> После обновления: " + repository.GetById(2) + "\n");
            }

            Console.WriteLine("[Операция 5] Специализированный поиск по категории (GetByCategory):");
            List<Product> laptops = repository.GetByCategory("Ноутбуки");
            foreach (Product p in laptops)
            {
                Console.WriteLine("  " + p);
            }
            Console.WriteLine();

            Console.WriteLine("[Операция 6] Удаление записи через Delete(4):");
            repository.Delete(4);
            Console.WriteLine("-> Запись с ID=4 удалена. Текущий список в JSON:");
            foreach (Product p in repository.GetAll())
            {
                Console.WriteLine("  " + p);
            }
            Console.WriteLine();

            Console.WriteLine("[Операция 7] Содержимое созданного JSON-файла:");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine(File.ReadAllText(dbFile));
            Console.WriteLine("--------------------------------------------------");

            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }
        }
    }
}
