using System;
using System.Linq;
using Xunit;

namespace MongoDB.SimpleRepository.Tests
{
    public class RepositoryTest
    {
        private readonly Repository<TestEntity, int> _repo;
        private const string ConnectionString = "mongodb://localhost/test";

        private static int Rand()
        {
            return new Random().Next(0, 1000);
        }

        private TestEntity Entity()
        {
            return new TestEntity(Rand());
        }


        public RepositoryTest()
        {
            MongoConnection.ConnectionString = ConnectionString;
            _repo = new Repository<TestEntity, int>();
        }

        [Fact]
        public void InsertTest()
        {
            var te = Entity();
            _repo.Insert(te);
            Assert.True(true);
            _repo.Delete(te);
        }

        [Fact]
        public void DeleteTest()
        {
            var te = Entity();
            _repo.Insert(te);

            _repo.Delete(te);
            var deletedTe = _repo.FindById(te.Id);
            Assert.Null(deletedTe);
        }

        [Fact]
        public void DeleteById()
        {
            var te = new TestEntity(Rand());
            _repo.Insert(te);

            _repo.Delete(te.Id);
            var deletedTe = _repo.FindById(te.Id);
            Assert.Null(deletedTe);
        }

        [Fact]
        public void FindByIdTest()
        {
            var newTe = new TestEntity(Rand());
            _repo.Insert(newTe);

            var foundTe = _repo.FindById(newTe.Id);
            Assert.NotNull(foundTe);
            _repo.Delete(newTe);
        }

        [Fact]
        public void UpdateTest()
        {
            var te = Entity();

            te.TestProperty = "VALUE";
            _repo.Insert(te);
            var updateProp = "UPDATE VALUE";
            te.TestProperty = updateProp;
            _repo.Update(te);
            var updatedTe = _repo.FindById(te.Id);
            Assert.Equal(updateProp, updatedTe.TestProperty);
            _repo.Delete(te);
        }

        [Fact]
        public void UpsertTest()
        {
            var te = Entity();
            te.TestProperty = "VALUE";

            _repo.UpSert(te);
            var upsertedTe = _repo.FindById(te.Id);
            Assert.NotNull(upsertedTe);

            var upsertProp = "UPSERT VALUE";
            upsertedTe.TestProperty = upsertProp;
            _repo.UpSert(upsertedTe);
            upsertedTe = _repo.FindById(te.Id);
            Assert.Equal(upsertProp, upsertedTe.TestProperty);
            _repo.Delete(te);
        }

        [Fact]
        public void SearchTest()
        {
            var te = Entity();
            te.TestProperty = "VALUE";
            _repo.Insert(te);

            var searchResults = _repo.Search(x => x.TestProperty.Contains("VAL")).ToList();
            Assert.Equal(te.Id, searchResults[0].Id);
            _repo.Delete(te);
        }

        [Fact]
        public void FindByNameTest()
        {
            var repo = new NamedRepository<NamedTestEntity, int>();
            var name = "Bob";
            var nte = new NamedTestEntity(Rand(), name);
            repo.Insert(nte);

            var foundNte = repo.FindByName(name);
            Assert.NotNull(foundNte);
            repo.Delete(nte);
        }

    }
}
