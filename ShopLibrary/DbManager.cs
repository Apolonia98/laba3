using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ShopLibrary
{
    // Одинак: глобальна точка доступу до бази даних товарів магазину
    public sealed class DbManager
    {
        private static DbManager instance;
        private static readonly object locker = new object();

        public static int CreatedCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string DbPath { get; private set; }

        private List<Product> products;

        private DbManager()
        {
            CreatedCount++;
            CreatedAt = DateTime.Now;
            string folder = Path.Combine(AppContext.BaseDirectory, "data");
            Directory.CreateDirectory(folder);
            DbPath = Path.Combine(folder, "products.json");
            Load();
            Logger.Instance.Log("DbManager: підключення до бази даних (" + products.Count + " товарів)");
        }

        public static DbManager Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new DbManager();
                    return instance;
                }
            }
        }

        private void Load()
        {
            if (File.Exists(DbPath))
            {
                products = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(DbPath));
                return;
            }
            products = new List<Product>
            {
                new Product { Id = 1, Name = "Хліб білий", Category = "Хлібобулочні", Price = 28, Quantity = 40 },
                new Product { Id = 2, Name = "Молоко 2,5%", Category = "Молочні", Price = 42, Quantity = 30 },
                new Product { Id = 3, Name = "Сир твердий", Category = "Молочні", Price = 95, Quantity = 15 },
                new Product { Id = 4, Name = "Яйця (10 шт.)", Category = "Яйця", Price = 64, Quantity = 25 },
                new Product { Id = 5, Name = "Яблука (1 кг)", Category = "Фрукти", Price = 35, Quantity = 50 },
                new Product { Id = 6, Name = "Кава мелена", Category = "Бакалія", Price = 180, Quantity = 12 },
                new Product { Id = 7, Name = "Цукор (1 кг)", Category = "Бакалія", Price = 38, Quantity = 20 },
                new Product { Id = 8, Name = "Вода 1,5 л", Category = "Напої", Price = 22, Quantity = 60 }
            };
            Save();
        }

        private void Save()
        {
            File.WriteAllText(DbPath, JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true }));
        }

        public List<Product> GetProducts()
        {
            lock (locker)
            {
                return products.Select(p => new Product { Id = p.Id, Name = p.Name, Category = p.Category, Price = p.Price, Quantity = p.Quantity }).ToList();
            }
        }

        public Product AddProduct(string name, string category, decimal price, int quantity)
        {
            lock (locker)
            {
                int id = products.Count == 0 ? 1 : products.Max(p => p.Id) + 1;
                Product product = new Product { Id = id, Name = name, Category = category, Price = price, Quantity = quantity };
                products.Add(product);
                Save();
                return product;
            }
        }

        public bool DeleteProduct(int id)
        {
            lock (locker)
            {
                int removed = products.RemoveAll(p => p.Id == id);
                if (removed > 0)
                    Save();
                return removed > 0;
            }
        }

        public bool Sell(int id, int quantity)
        {
            lock (locker)
            {
                Product product = products.FirstOrDefault(p => p.Id == id);
                if (product == null || quantity <= 0 || product.Quantity < quantity)
                    return false;
                product.Quantity -= quantity;
                Save();
                return true;
            }
        }
    }
}
