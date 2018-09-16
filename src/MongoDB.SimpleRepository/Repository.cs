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
        protected IMongoCollection<TEntity> collection;

        public Repository()
        {
            var mongoUrl = new MongoUrl(MongoConnection.ConnectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);
            SetCollection();
        }

        public Repository(string connectionString, string collectionName = null)
        {
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);
            SetCollection(collectionName);
        }

        private void SetCollection(string name = null)
        {
            if(name == null)
            {
                name = typeof(TEntity).Name;
            }

            if (!CollectionExistsAsync(name).Result)
            {
                Db.CreateCollection(name);
            }

            collection = Db.GetCollection<TEntity>(name);

        }
        public async Task<bool> CollectionExistsAsync(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);

            var collections = await Db.ListCollectionsAsync(
                new ListCollectionsOptions {
                    Filter = filter
                }
            );

            return await collections.AnyAsync();
        }


        public IMongoCollection<TEntity> Collection()
        {
            return collection;
        }

        public async Task Insert(TEntity entity)
        {
            await collection.InsertOneAsync(entity);
        }

        public async Task<uint> Update(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            var result = await collection.ReplaceOneAsync(filter, entity);
            return (uint) result.ModifiedCount;
        }

        public async Task Upsert(TEntity entity)
        {
            if (Equals(entity.Id, default(TId)))
            {
                await Insert(entity);
            }
            else
            {
                var updated = await Update(entity);
                if (updated == 0)
                {
                    await Insert(entity);
                }
            }
        }

        public void Delete(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            collection.DeleteOne(filter);
        }

        public void Delete(TId id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            collection.DeleteOne(filter);
        }

        public IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate)
        {
            return collection.AsQueryable().Where(predicate.Compile());
        }

        public IEnumerable<TEntity> GetAll()
        {
            return collection.AsQueryable();
        }

        public TEntity FindById(TId id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return collection.Find(filter).FirstOrDefault();
        }
    }
}
