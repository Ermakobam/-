using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;

namespace Shop.DAL.Repositories
{
    public class JsonRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly string filePath;
        protected List<T> items;
        private int nextId;

        public JsonRepository(string fileName)
        {
            filePath = fileName;
            items = LoadFromFile();
            nextId = 1;
            foreach (T item in items)
            {
                if (item.Id >= nextId)
                {
                    nextId = item.Id + 1;
                }
            }
        }

        private List<T> LoadFromFile()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<T>();
                }

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<T>();
                }

                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        private void SaveToFile()
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

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

        public virtual List<T> GetAll()
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
            else if (entity.Id >= nextId)
            {
                nextId = entity.Id + 1;
            }
            items.Add(entity);
            SaveToFile();
        }

        public virtual void Update(T entity)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == entity.Id)
                {
                    items[i] = entity;
                    SaveToFile();
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
                SaveToFile();
            }
        }
    }
}
