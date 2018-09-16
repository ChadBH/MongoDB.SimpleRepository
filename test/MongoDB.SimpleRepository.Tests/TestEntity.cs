namespace MongoDB.SimpleRepository.Tests
{
    public class TestEntity : Entity<int>
    {
        public string TestProperty { get; set; }

        public TestEntity(int id):base(id) { }
    }
}
