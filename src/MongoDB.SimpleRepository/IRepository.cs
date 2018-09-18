using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.SimpleRepository
{
    public interface IRepository<TEntity, in TId>
    {
        Task InsertAsync(TEntity entity);
        Task InsertAsync(IEnumerable<TEntity> entities);
        Task<uint> UpdateAsync(TEntity entity);
        Task UpsertAsync(TEntity entity);
        Task DeleteAsync(TId id);
        Task DeleteAsync(IEnumerable<TId> ids);
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        Task<TEntity> FindByIdAsync(TId id);
    }
}