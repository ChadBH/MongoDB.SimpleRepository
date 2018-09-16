using MongoDB.Driver;

namespace MongoDB.SimpleRepository
{
    public class NamedRepository<TEntity, TId> : Repository<TEntity, TId> where TEntity : NamedEntity<TId>
    {
        public TEntity FindByName(string name)
        {
            var filter = Builders<TEntity>.Filter.Eq("Name", name);
            return collection.Find(filter).FirstOrDefault();
        }

    }
}
