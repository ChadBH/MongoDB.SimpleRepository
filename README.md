# MongoDB.SimpleRepository
[![NuGet version (MongoDB.Repository.Core)](https://img.shields.io/nuget/v/MongoDB.Repository.Core.svg?style=flat-square)](https://www.nuget.org/packages/MongoDB.Repository.Core/)

Repository implementation for .net core MongoDB driver. 

```
Install-Package MongoDB.Repository.Core -Version 2.0.0 
```

### Supports
* Any type for id, like Guid or int. ObjectId or BsonId are not required in your classes. Keep your POCO objects plain!
* Connection pooling. Takes advantage of MongoDb.Driver's support for thread-safe static database and collection references.
* Bulk add or delete.

### Usage

1. Name one of the fields "Id" in your model. It doesn't need to be an ObjectId or have Bson attributes. Any type except for a list or array will do as an Id. 
```csharp
    public class Record
    {
        public Guid Id {get; set;}
        public string Name { get; set; }
    }
```

2. Your repository takes two type parameters--the type of your record and the type of the Id--and then your connection string.
```csharp
    var repo = new Repository<Record, Guid>(connectionString);
```

You can optionally pass in the collection name, otherwise the type name will be used. The collection will be created if it doesn't already exist.

```csharp
    var repo = new Repository<Record, Guid>(connectionString, "Records");
```

### Examples
FindById:
```csharp
    var record = await repo.FindByIdAsync(new Guid("134c24bf-e1c8-4ec9-bd7e-ebbe211ebb72"));
```

Upsert accepts an optional IEqualityComparer, which will test whether there are actually any changes between the record to upsert and the existing record. This can greatly improve speed, since it only writes when it needs to.

```csharp
    public class RecordComparer : IEqualityComparer<Record>
    {
        public bool Equals(Record x, Record y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            return x.Name == y.Name;
        }

        public int GetHashCode(Record obj)
        {
            return obj.Name?.GetHashCode() ?? 0;
        }
    }
    
    //Implementation
    repo.Upsert(myRecord, new RecordComparer());
```
