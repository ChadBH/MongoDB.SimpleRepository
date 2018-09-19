using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private static TestEntity Entity()
        {
            return new TestEntity(Rand());
        }

        private static List<TestEntity> Entities(int count = 10)
        {
            return Enumerable
                .Range(0, count)
                .Select(a => Entity())
                .ToList();
        }


        public RepositoryTest()
        {
            _repo = new Repository<TestEntity, int>(ConnectionString);
            _repo.Empty();
        }

        [Fact]
        public async Task InsertTest()
        {
            var te = Entity();
            await _repo.InsertAsync(te);
            Assert.True(true);
            await _repo.DeleteAsync(te.Id);
        }

        [Fact]
        public async Task DeleteTest()
        {
            var te = Entity();
            await _repo.InsertAsync(te);

            await _repo.DeleteAsync(te.Id);
            var deletedTe = await _repo.FindByIdAsync(te.Id);
            Assert.Null(deletedTe);
        }

        [Fact]
        public async Task DeleteById()
        {
            var te = new TestEntity(Rand());
            await _repo.InsertAsync(te);

            await _repo.DeleteAsync(te.Id);
            var deletedTe = await _repo.FindByIdAsync(te.Id);
            Assert.Null(deletedTe);
        }

        [Fact]
        public async Task FindByIdTest()
        {
            var newTe = new TestEntity(Rand());
            await _repo.InsertAsync(newTe);

            var foundTe = _repo.FindByIdAsync(newTe.Id);
            Assert.NotNull(foundTe);
            await _repo.DeleteAsync(newTe.Id);
        }

        [Fact]
        public async Task UpdateTest()
        {
            var te = Entity();

            te.TestProperty = "VALUE";
            await _repo.InsertAsync(te);
            var updateProp = "UPDATE VALUE";
            te.TestProperty = updateProp;
            await _repo.UpdateAsync(te);
            var updatedTe = await _repo.FindByIdAsync(te.Id);
            Assert.Equal(updateProp, updatedTe.TestProperty);
            await _repo.DeleteAsync(te.Id);
        }

        [Fact]
        public async Task UpsertTest()
        {
            var te = Entity();
            te.TestProperty = "VALUE";

            await _repo.UpsertAsync(te);
            var upsertedTe = await _repo.FindByIdAsync(te.Id);
            Assert.NotNull(upsertedTe);

            var upsertProp = "UPSERT VALUE";
            upsertedTe.TestProperty = upsertProp;
            await _repo.UpsertAsync(upsertedTe);
            upsertedTe = await _repo.FindByIdAsync(te.Id);
            Assert.Equal(upsertProp, upsertedTe.TestProperty);
            await _repo.DeleteAsync(te.Id);
        }

        [Fact]
        public async Task SearchTest()
        {
            var te = Entity();
            te.TestProperty = "VALUE";
            await _repo.InsertAsync(te);

            var searchResults = _repo.Search(x => x.TestProperty.Contains("VAL")).ToList();
            Assert.Equal(te.Id, searchResults[0].Id);
            await _repo.DeleteAsync(te.Id);
        }

        [Fact]
        public async Task FindByNameTest()
        {
            var repo = new NamedRepository<NamedTestEntity, int>(ConnectionString);
            var name = "Bob";
            var nte = new NamedTestEntity(Rand(), name);
            await repo.InsertAsync(nte);

            var foundNte = repo.FindByName(name);
            Assert.NotNull(foundNte);
            await repo.DeleteAsync(nte.Id);
        }

        [Fact]
        public async Task AddDeleteMany()
        {
            var inserts = Entities();

            await _repo.InsertAsync(inserts);

            foreach (var insert in inserts)
            {
                var found = await _repo.FindByIdAsync(insert.Id);
                Assert.NotNull(found);
            }

            await _repo.DeleteAsync(inserts.Select(a => a.Id));

            foreach (var insert in inserts)
            {
                var found = await _repo.FindByIdAsync(insert.Id);
                Assert.Null(found);
            }
        }
    }
}