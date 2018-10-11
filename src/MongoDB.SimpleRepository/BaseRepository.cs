using MongoDB.Driver;

namespace MongoDB.SimpleRepository
{
    public abstract class BaseRepository
    {
        protected static IMongoDatabase Db;

        /// <summary>
        /// Lock to protect against concurrent registering of duplicate class maps.
        /// </summary>
        protected readonly object RegisterLock = new object();

        protected BaseRepository(string connectionString = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "mongodb://localhost/test";
            }

            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            Db = client.GetDatabase(mongoUrl.DatabaseName);
        }
    }
}