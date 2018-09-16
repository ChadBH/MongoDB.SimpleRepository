using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.SimpleRepository
{
    public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        Task InsertAsync(TEntity entity);
        Task<uint> UpdateAsync(TEntity entity);
        Task UpsertAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        Task<TEntity> FindByIdAsync(TId id);
    }
}
