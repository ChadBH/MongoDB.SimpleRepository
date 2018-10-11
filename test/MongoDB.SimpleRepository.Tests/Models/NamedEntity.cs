namespace MongoDB.SimpleRepository.Tests.Models
{
    public abstract class NamedEntity<TId> 
    {
        public TId Id { get; set; }

        public string Name { get; set; }

        protected NamedEntity(TId id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
