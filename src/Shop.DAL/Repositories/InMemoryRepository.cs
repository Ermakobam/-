using System.Collections.Generic;
using Shop.Core.Entities;
using Shop.DAL.Interfaces;

namespace Shop.DAL.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> items = new List<T>();
        private int nextId = 1;

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
            items.Add(entity);
        }

        public virtual void Update(T entity)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == entity.Id)
                {
                    items[i] = entity;
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
            }
        }
    }
}
