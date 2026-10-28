using System.Collections.Generic;
using Shop.Core.Entities;

namespace Shop.DAL.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T? GetById(int id);
        List<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
