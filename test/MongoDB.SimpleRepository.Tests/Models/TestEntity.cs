namespace MongoDB.SimpleRepository.Tests.Models
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
