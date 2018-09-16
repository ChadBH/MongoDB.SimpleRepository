namespace MongoDB.SimpleRepository.Tests
{
    public class NamedTestEntity : NamedEntity<int>
    {
        public string TestProperty { get; set; }

        public NamedTestEntity(int id, string value) : base(id, value)
        {
            TestProperty = value;
        }
    }
}
