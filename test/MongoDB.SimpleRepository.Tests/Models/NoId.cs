using System;

namespace MongoDB.SimpleRepository.Tests.Models
{
    public class NoId
    {
        public Guid ArbitraryIdentifier { get; set; }

        public string Value { get; set; }

        public NoId()
        {
            ArbitraryIdentifier = Guid.NewGuid();
        }

        public NoId(string value) : this()
        {
            Value = value;
        }

        public NoId(Guid id, string value) : this(value)
        {
            ArbitraryIdentifier = id;
        }
    }
}
