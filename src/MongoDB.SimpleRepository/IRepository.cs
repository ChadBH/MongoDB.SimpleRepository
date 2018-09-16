using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MongoDB.SimpleRepository
{
    public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void UpSert(TEntity entity);
        void Delete(TEntity entity);
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        TEntity FindById(TId id);
        IMongoCollection<TEntity> Collection();
    }
}
