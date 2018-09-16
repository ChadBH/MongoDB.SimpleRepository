using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public Repository(string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);
            SetCollection();
        }

        private void SetCollection()
        {
            collection = Db.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public void SetCollectionByName(string collectionName)
        {
            collection = Db.GetCollection<TEntity>(collectionName);
        }

        public IMongoCollection<TEntity> Collection()
        {
            return collection;
        }

        public void Insert(TEntity entity)
        {
            collection.InsertOne(entity);
        }

        public void Update(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            collection.ReplaceOne(filter, entity);
        }

        public void UpSert(TEntity entity)
        {
            if (entity.Id == null)
                Insert(entity);
            else
                Update(entity);
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
