using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDB.SimpleRepository
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId> 
    {
        protected static IMongoDatabase Db;
        protected static IMongoCollection<TEntity> Collection;
        private static MemberInfo _idMemberInfo;

        public Repository(
            string connectionString,
            string collectionName = null,
            string idMemberName = null
        ){
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);

            if (string.IsNullOrWhiteSpace(idMemberName))
            {
                idMemberName = "Id";
            }

            _idMemberInfo = GetIdField(idMemberName);

            if (_idMemberInfo == null)
            {
                var typeName = typeof(TEntity).Name;

                throw new ArgumentException(
                    $"{typeName} does not have a field or property named {idMemberName}.",
                    nameof(idMemberName)
                );
            }

            var idType = typeof(TId);
            var reflectedIdType = GetType(_idMemberInfo);

            if (idType != reflectedIdType)
            {
                if (reflectedIdType != null)
                {
                    throw new ArgumentException(
                        $"{idMemberName} is a {reflectedIdType.Name} but the TId type argument for this repository is {idType}.",
                        nameof(idMemberName)
                    );
                }

                throw new ArgumentException(
                    $"{idMemberName} is missing.",
                    nameof(idMemberName)
                );

            }

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
            var id = GetIdValue(entity);
            var result = await Collection.ReplaceOneAsync(Filter(id), entity);
            return (uint) result.ModifiedCount;
        }

        public async Task UpsertAsync(TEntity entity)
        {
            var id = GetIdValue(entity);

            if (Equals(id, default(TId)))
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

        public async Task DeleteAsync(TId id)
        {
            await Collection.DeleteOneAsync(Filter(id));
        }

        public async Task Delete(TId id)
        {
            await Collection.DeleteOneAsync(Filter(id));
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
            var result = await Collection.FindAsync(Filter(id));
            return result.FirstOrDefault();
        }

        public static Type GetType(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    return typeof(TId);
            }
        }

        public static TId GetValue(MemberInfo memberInfo, TEntity forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return (TId) ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return (TId) ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    return default(TId);
            }
        }

        public TId GetIdValue(TEntity forObject)
        {
            return GetValue(_idMemberInfo, forObject);
        }

        private static MemberInfo GetIdField(string propertyName)
        {
            var type = typeof(TEntity);

            var info = type.GetProperty(propertyName);

            if (info == null)
            {
                return type.GetField(propertyName);
            }

            return info;
        }

        private static FilterDefinition<TEntity> Filter(TId id)
        {
            return Builders<TEntity>.Filter.Eq("_id", id);
        }
    }
}
