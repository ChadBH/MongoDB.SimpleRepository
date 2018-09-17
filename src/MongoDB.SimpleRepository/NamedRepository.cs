﻿using MongoDB.Driver;

namespace MongoDB.SimpleRepository
{
    public class NamedRepository<TEntity, TId> : Repository<TEntity, TId> where TEntity : NamedEntity<TId>
    {
        public TEntity FindByName(string name)
        {
            var filter = Builders<TEntity>.Filter.Eq("Name", name);
            return Collection.Find(filter).FirstOrDefault();
        }

        public NamedRepository(
            string connectionString, 
            string collectionName = null, 
            string idMemberName = null
        ) : base(connectionString, collectionName, idMemberName) { }
    }
}
