using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace MongoDB.SimpleRepository
{
    public class Repository<TEntity, TId> : BaseRepository, IRepository<TEntity, TId> 
    {
        protected static IMongoCollection<TEntity> Collection;

        // ReSharper disable once StaticMemberInGenericType
        private static MemberInfo _idMemberInfo;

        public Repository(
            string connectionString = null,
            string collectionName = null,
            string idMemberName = "Id"
        ):base(connectionString){

            _idMemberInfo = GetIdField(idMemberName);

            if (_idMemberInfo == null)
            {
                var typeName = typeof(TEntity).Name;

                throw new ArgumentException(
                    $"{typeName} does not have a field or property named {idMemberName}.",
                    nameof(idMemberName)
                );
            }

            var entityType = typeof(TEntity);

            if (idMemberName != "Id")
            {
                var typeIsRegistered = BsonClassMap.IsClassMapRegistered(entityType);

                if (!typeIsRegistered)
                {
                    lock (RegisterLock)
                    {
                        BsonClassMap.RegisterClassMap<TEntity>(cm =>
                        {
                            cm.AutoMap();
                            cm.SetIgnoreExtraElements(true);
                            cm.MapIdMember(_idMemberInfo);
                        });
                    }
                }
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
                collectionName = entityType.Name;
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

        public virtual TEntity FindById(TId id)
        {
            return Collection.Find(Filter(id)).FirstOrDefault();
        }

        public virtual async Task<TEntity> FindByIdAsync(TId id)
        {
            var result = await Collection.FindAsync(Filter(id));
            return result.FirstOrDefault();
        }

        public virtual void Insert(TEntity entity)
        {
            Collection.InsertOne(entity);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            Collection.BulkWrite(entities.Select(e => new InsertOneModel<TEntity>(e)));
        }

        public virtual async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            await Collection.BulkWriteAsync(entities.Select(e => new InsertOneModel<TEntity>(e)));
        }

        public void Update(TEntity entity)
        {
            var id = GetIdValue(entity);
            Collection.ReplaceOne(Filter(id), entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            var id = GetIdValue(entity);
            await Collection.ReplaceOneAsync(Filter(id), entity);
        }

        public void Upsert(
            TEntity entity,
            IEqualityComparer<TEntity> comparer = null
        ){
            var id = GetIdValue(entity);
            if (Equals(id, default(TId)))
            {
                Insert(entity);
            }
            else
            {
                var found = FindById(id);
                if (found == null)
                {
                    Insert(entity);
                }
                else
                {
                    if (comparer == null || !comparer.Equals(found, entity))
                    {
                        Update(entity);
                    }
                }
            }
        }

        public virtual async Task UpsertAsync(
            TEntity entity, 
            IEqualityComparer<TEntity> comparer = null
        ){
            var id = GetIdValue(entity);

            if (Equals(id, default(TId)))
            {
                await InsertAsync(entity);
            }
            else
            {
                var found = await FindByIdAsync(id);
                if (found == null)
                {
                    await InsertAsync(entity);
                }
                else
                {
                    if(comparer == null || !comparer.Equals(found, entity))
                    await UpdateAsync(entity);
                }
            }
        }

        public virtual void Delete(TId id)
        {
            Collection.DeleteOne(Filter(id));
        }

        public virtual async Task DeleteAsync(TId id)
        {
            await Collection.DeleteOneAsync(Filter(id));
        }

        public virtual void Delete(IEnumerable<TId> ids)
        {
            Collection.BulkWrite(ids.Select(e => new DeleteOneModel<TEntity>(Filter(e))));
        }

        public virtual async Task DeleteAsync(IEnumerable<TId> ids)
        {
            await Collection.BulkWriteAsync(ids.Select(e => new DeleteOneModel<TEntity>(Filter(e))));
        }

        public virtual IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.AsQueryable().Where(predicate.Compile());
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return Collection.AsQueryable();
        }

        public virtual void Empty()
        {
            Collection.DeleteMany(a => true);
        }

        public virtual async Task EmptyAsync()
        {
            await Collection.DeleteManyAsync(a => true);
        }

        private static Type GetType(MemberInfo memberInfo)
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

        private static TId GetValue(MemberInfo memberInfo, TEntity forObject)
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

        protected TId GetIdValue(TEntity forObject)
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

        protected static FilterDefinition<TEntity> FilterIn(IEnumerable<TId> ids)
        {
            return Builders<TEntity>.Filter.In("_id", ids);
        }

        protected static FilterDefinition<TEntity> Filter(TId id)
        {
            return Builders<TEntity>.Filter.Eq("_id", id);
        }
    }
}
