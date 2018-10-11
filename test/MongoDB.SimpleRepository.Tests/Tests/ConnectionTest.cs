using System;
using MongoDB.Driver;
using MongoDB.SimpleRepository.Tests.Models;
using Xunit;

namespace MongoDB.SimpleRepository.Tests.Tests
{
    public class ConnectionTest
    {
        [Fact]
        public void RepositorySecondConstTest()
        {
            var repo = new Repository<TestEntity, int>();
            Assert.True(true);
        }

        [Fact]
        public void NonExistantConnection()
        {
            try
            {
                var repo = new Repository<TestEntity, int>("nonexistantcollection");
                Assert.False(true, "Should fail to connect");
            }
            catch (Exception e)
            {
                var exceptionType = e.GetType();
                var expected = typeof(MongoConfigurationException);

                Assert.Equal(expected, exceptionType);
            }
        }
    }
}
