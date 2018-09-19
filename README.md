# MongoDB.SimpleRepository

Repository implementation for .net core MongoDB driver. 

### Supports
* Generic id fields, like Guid or int. ObjectId or BsonId are not required in your classes. Keep your POCO objects plain!
* Connection pooling. Takes advantage of MongoDb.Driver's support for thread-safe static database and collection references.
* Bulk add or delete.

### Nuget package MongoDB.Repository.Core`

### Usage

1. Name one of the fields "Id" in your model. It doesn't need to be an ObjectId or have Bson attributes. Any type except for a list or array will do as an Id. 
```csharp
    public class Record
    {
        public Guid Id {get; set;}
        public string Name { get; set; }
    }
```

2. Your repository. Takes two type parameters--the type of your record and the type of the Id--and then your connection string.
```csharp
    var repo = new Repository<Record, Guid>(connectionString);
```

### Available methods
FindByIdAsync
```csharp
    var record = await repo.FindByIdAsync(new Guid("134c24bf-e1c8-4ec9-bd7e-ebbe211ebb72"));
```
