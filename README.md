# MongoDB.SimpleRepository

Repository implementation for .net core MongoDB driver. 

### Supports
* Generic id fields, like Guid or int. ObjectId or BsonId are not required in your classes. Keep your POCO objects plain!
* No id inheritance, either. Typically your model has to inherit an id from another class, creating a dependency back to the repository. Yuck!
* Connection pooling. Takes advantage of MongoDb.Driver's support for thread-safe static database and collection references.

### Nuget package MongoDB.Repository.Core`

### Create Generic Repository
```csharp
    Repository<YourClass> repo = new Repository<YourClass, int>(connectionString);
    YourClass yourClass = new YourClass();
    yourClass.TestProperty = "Value";
    repo.Insert(yourClass);
```

### Creating Your Own Repositories
```csharp
    public abstract class NamedEntity
    {
        public Guid Id {get; set;}
        public string Name { get; set; }
    }
```
```csharp
    public class NamedRepository<TEntity> : Repository<TEntity, Guid> where TEntity : NamedEntity
    {
        public TEntity FindByName(string name)
        {
            var filter = Builders<TEntity>.Filter.Eq("Name", name);
            return collection.Find(filter).FirstOrDefault();
        }
    }
```
