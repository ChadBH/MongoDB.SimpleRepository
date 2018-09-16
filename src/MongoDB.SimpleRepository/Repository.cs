using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.SimpleRepository
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        protected IMongoDatabase Db;
        protected IMongoCollection<TEntity> Collection;

        public Repository():this(MongoConnection.ConnectionString) {}

        public Repository(string connectionString, string collectionName = null)
        {
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);

            if(string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = typeof(TEntity).Name;
            }

            var filter = new BsonDocument("name", collectionName);

            var collections = Db.ListCollections(
                new ListCollectionsOptions {
                    Filter = filter
                }
            );

            if (!collections.Any())
            {
                Db.CreateCollection(collectionName);
            }

            Collection = Db.GetCollection<TEntity>(collectionName);

        }

        public async Task InsertAsync(TEntity entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        public async Task<uint> UpdateAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            var result = await Collection.ReplaceOneAsync(filter, entity);
            return (uint) result.ModifiedCount;
        }

        public async Task UpsertAsync(TEntity entity)
        {
            if (Equals(entity.Id, default(TId)))
            {
                await InsertAsync(entity);
            }
            else
            {
                var updated = await UpdateAsync(entity);
                if (updated == 0)
                {
                    await InsertAsync(entity);
                }
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task Delete(TId id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await Collection.DeleteOneAsync(filter);
        }

        public IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.AsQueryable().Where(predicate.Compile());
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Collection.AsQueryable();
        }

        public async Task<TEntity> FindByIdAsync(TId id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            var result = await Collection.FindAsync(filter);
            return result.FirstOrDefault();
        }
    }
}
