namespace MongoDB.SimpleRepository
{
    public abstract class Entity<T>
    {
        public T Id { get; set; }

        protected Entity() { }

        protected Entity(T id)
        {
            Id = id;
        }
    }
}
