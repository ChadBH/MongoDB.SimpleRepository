using Xunit;

namespace MongoDB.SimpleRepository.Tests
{
    public class ConnectionTest
    {
        private readonly string _connectionString;

        public ConnectionTest()
        {
            _connectionString = Config.Settings["ConnectionString"];
        }

        [Fact]
        public void RepositoryFirstConstTest() 
        {
            var repo = new Repository<TestEntity, int>(_connectionString);
            Assert.True(true);
        }

        [Fact]
        public void RepositorySecondConstTest()
        {
            var repo = new Repository<TestEntity, int>(_connectionString);
            Assert.True(true);
        }
    }
}
