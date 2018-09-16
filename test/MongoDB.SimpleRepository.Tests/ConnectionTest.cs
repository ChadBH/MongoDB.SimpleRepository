using Xunit;

namespace MongoDB.SimpleRepository.Tests
{
    public class ConnectionTest
    {
        private const string ConnString = "mongodb://localhost/test";

        [Fact]
        public void RepositoryFirstConstTest() 
        {
            MongoConnection.ConnectionString = ConnString;
            var repo = new Repository<TestEntity, int>();
            Assert.True(true);
        }

        [Fact]
        public void RepositorySecondConstTest()
        {
            var repo = new Repository<TestEntity, int>(ConnString);
            Assert.True(true);
        }
    }
}
