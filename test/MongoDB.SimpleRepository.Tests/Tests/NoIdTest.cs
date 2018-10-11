using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.SimpleRepository.Tests.Models;
using MongoDB.SimpleRepository.Tests.Repositories;
using Xunit;

namespace MongoDB.SimpleRepository.Tests.Tests
{
    public class NoIdTest : TestBase
    {
        private readonly IRepository<NoId, Guid> _repo;

        public NoIdTest()
        {
            _repo = new NoIdRepo();
            _repo.Empty();
        }

        [Fact]
        public async Task InsertTest()
        {
            var te = Entity();
            await _repo.InsertAsync(te);
            Assert.True(true);
            await _repo.DeleteAsync(te.ArbitraryIdentifier);
        }

        [Fact]
        public async Task DeleteTest()
        {
            var te = Entity();
            await _repo.InsertAsync(te);

            await _repo.DeleteAsync(te.ArbitraryIdentifier);
            var deletedTe = await _repo.FindByIdAsync(te.ArbitraryIdentifier);
            Assert.Null(deletedTe);
        }

        [Fact]
        public async Task DeleteById()
        {
            var te = Entity();
            await _repo.InsertAsync(te);

            await _repo.DeleteAsync(te.ArbitraryIdentifier);
            var deletedTe = await _repo.FindByIdAsync(te.ArbitraryIdentifier);
            Assert.Null(deletedTe);
        }

        [Fact]
        public async Task FindByIdTest()
        {
            var newTe = Entity();
            await _repo.InsertAsync(newTe);

            var foundTe = _repo.FindByIdAsync(newTe.ArbitraryIdentifier);
            Assert.NotNull(foundTe);
            await _repo.DeleteAsync(newTe.ArbitraryIdentifier);
        }

        [Fact]
        public async Task UpdateTest()
        {
            var te = Entity();

            te.Value = "VALUE";
            await _repo.InsertAsync(te);
            var updateProp = "UPDATE VALUE";
            te.Value = updateProp;
            await _repo.UpdateAsync(te);
            var updatedTe = await _repo.FindByIdAsync(te.ArbitraryIdentifier);
            Assert.Equal(updateProp, updatedTe.Value);
            await _repo.DeleteAsync(te.ArbitraryIdentifier);
        }

        [Fact]
        public async Task UpsertTest()
        {
            var te = Entity();
            te.Value = "VALUE";

            await _repo.UpsertAsync(te);
            var upsertedTe = await _repo.FindByIdAsync(te.ArbitraryIdentifier);
            Assert.NotNull(upsertedTe);

            var upsertProp = "UPSERT VALUE";
            upsertedTe.Value = upsertProp;
            await _repo.UpsertAsync(upsertedTe);
            upsertedTe = await _repo.FindByIdAsync(te.ArbitraryIdentifier);
            Assert.Equal(upsertProp, upsertedTe.Value);
            await _repo.DeleteAsync(te.ArbitraryIdentifier);
        }

        [Fact]
        public async Task SearchTest()
        {
            var te = Entity();
            te.Value = "VALUE";
            await _repo.InsertAsync(te);

            var searchResults = _repo.Search(x => x.Value.Contains("VAL")).ToList();
            Assert.Equal(te.ArbitraryIdentifier, searchResults[0].ArbitraryIdentifier);
            await _repo.DeleteAsync(te.ArbitraryIdentifier);
        }

        [Fact]
        public async Task AddDeleteMany()
        {
            var inserts = Entities();

            await _repo.InsertAsync(inserts);

            foreach (var insert in inserts)
            {
                var found = await _repo.FindByIdAsync(insert.ArbitraryIdentifier);
                Assert.NotNull(found);
            }

            await _repo.DeleteAsync(inserts.Select(a => a.ArbitraryIdentifier));

            foreach (var insert in inserts)
            {
                var found = await _repo.FindByIdAsync(insert.ArbitraryIdentifier);
                Assert.Null(found);
            }
        }

        private static NoId Entity()
        {
            return new NoId(Guid.NewGuid().ToString());
        }

        private static List<NoId> Entities(int count = 10)
        {
            return Enumerable
                .Range(0, count)
                .Select(a => Entity())
                .ToList();
        }
    }
}
