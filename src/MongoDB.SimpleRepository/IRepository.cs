﻿using System;
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

        void Upsert(TEntity entity, IEqualityComparer<TEntity> comparer = null);
        Task UpsertAsync(TEntity entity, IEqualityComparer<TEntity> comparer = null);

        void Delete(TId id);
        Task DeleteAsync(TId id);

        void Delete(IEnumerable<TId> ids);
        Task DeleteAsync(IEnumerable<TId> ids);

        void Empty();
        Task EmptyAsync();

        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
    }
}