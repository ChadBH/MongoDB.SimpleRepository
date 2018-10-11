# MongoDB.SimpleRepository
[![NuGet version (MongoDB.Repository.Core)](https://img.shields.io/nuget/v/MongoDB.Repository.Core.svg?style=flat-square)](https://www.nuget.org/packages/MongoDB.Repository.Core/)

Repository implementation for .net core MongoDB driver. 

```
Install-Package MongoDB.Repository.Core -Version 2.0.0 
```

### Supports
* Any type for \_id, like Guid or int. ObjectId or BsonId are not required in your classes. Keep your POCO objects plain!
* Connection pooling. Takes advantage of MongoDb.Driver's support for thread-safe static database and collection references.
* Bulk add or delete.
* Asynchronous CRUD methods.

### Usage

1. Decide which field is going to be your "Id" field. It doesn't need to be an ObjectId or have Bson attributes. Any type except for a list or array will do as the \_id. It also does not have to be named "Id", and does not require attributes to designate it as such. Your models can be true POCO classes without references to MongoDB.Driver!
```csharp
    public class Record
    {
        public Guid MyId {get; set;}
        public string Name { get; set; }
    }
```
2. Your repository expects two type parameters--the type of your record and the type of the \_id member. The arguments are the connection string, the name of the collection, and the name of the \_id member. All fields are optional. The connection string will default to your localhost mongod instance. The collection name defaults to the type name, and the default name it expects for the \_id member is "Id".
```csharp
    var repo = new Repository<Record, Guid>(connectionString, collectionName, idName);
```
### Examples
FindById:
```csharp
    var record = await repo.FindByIdAsync(new Guid("134c24bf-e1c8-4ec9-bd7e-ebbe211ebb72"));
```

Upsert accepts an optional IEqualityComparer, which will test whether there are actually any changes between the record to upsert and the existing record. This can greatly improve speed, since it will only write the record back when it sees it has a change.

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
