namespace MongoDB.SimpleRepository.Tests
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string TestProperty { get; set; }

        public TestEntity(int id)
        {
            Id = id;
        }
    }
}
