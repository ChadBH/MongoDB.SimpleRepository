using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.SimpleRepository
{
    public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        Task Insert(TEntity entity);
        Task<uint> Update(TEntity entity);
        Task Upsert(TEntity entity);
        void Delete(TEntity entity);
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        TEntity FindById(TId id);
        IMongoCollection<TEntity> Collection();
    }
}
