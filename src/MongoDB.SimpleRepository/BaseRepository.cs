using MongoDB.Driver;

namespace MongoDB.SimpleRepository
{
    public abstract class BaseRepository
    {
        protected static IMongoDatabase Db;

        protected BaseRepository(string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);
        }
    }
}