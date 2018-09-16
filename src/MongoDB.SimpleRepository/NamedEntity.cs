namespace MongoDB.SimpleRepository
{
    public abstract class NamedEntity<TId> : Entity<TId>
    {
        public string Name { get; set; }

        protected NamedEntity(TId id, string name) : base(id)
        {
            Name = name;
        }
    }
}
