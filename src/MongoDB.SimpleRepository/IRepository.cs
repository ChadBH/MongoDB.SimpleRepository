using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.SimpleRepository
{
    public interface IRepository<TEntity, in TId>
    {
        TEntity FindById(TId id);
        Task<TEntity> FindByIdAsync(TId id);

        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);

        void Insert(IEnumerable<TEntity> entities);
        Task InsertAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);

        void Upsert(TEntity entity);
        Task UpsertAsync(TEntity entity);

        void Delete(TId id);
        Task DeleteAsync(TId id);

        void Delete(IEnumerable<TId> ids);
        Task DeleteAsync(IEnumerable<TId> ids);

        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
    }
}