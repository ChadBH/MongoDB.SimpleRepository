using System;
using MongoDB.SimpleRepository.Tests.Models;

namespace MongoDB.SimpleRepository.Tests.Repositories
{
    public class NoIdRepo : Repository<NoId, Guid>
    {
        public NoIdRepo():base(null, null, "ArbitraryIdentifier") {}
    }
}
