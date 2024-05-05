namespace Apito.Services;

using MongoDB.Bson;
using MongoDB.Driver;

public class MongoCrud
{
    private readonly IMongoClient mongoClient;
    public MongoCrud(IMongoClient client)
    {
        mongoClient = client;
        Databases = [];
        if (client.Settings.Credential != null)
        {
            foreach (var dbName in mongoClient.ListDatabaseNames().ToEnumerable())
            {
                IMongoDatabase? db = null;
                Databases?.Add(dbName, db);
            }
        }
    }

    // PRIVATE

    private Dictionary<string, IMongoDatabase?>? Databases { get; set; }
    private bool GetDatabase(string dbName)
    {
        if (!Databases!.ContainsKey(dbName))
            return false;
        Databases![dbName] ??= mongoClient.GetDatabase(dbName);
        return true;
    }
    
    // PUBLIC

    public List<string>? GetCollectionNames(string dbName)
    {
        if (!GetDatabase(dbName))
            return null;
        return Databases![dbName]?.ListCollectionNames().ToList();
    }
    public async Task<List<string>?> GetCollectionNamesAsync(string dbName)
    {
        if (!GetDatabase(dbName))
            return null;
        return await Databases![dbName]?.ListCollectionNames().ToListAsync()!;
    }
    
    public IMongoCollection<BsonDocument>? GetCollection(string name, string dbName)
    {
        var collection = GetCollectionNames(dbName);
        if (collection == null)
            return null;
        if (!collection!.Contains(name))
            return null;
        //var items = await collection.Find(_ => true).ToListAsync();
        return Databases![dbName]?.GetCollection<BsonDocument>(name)!;
    }
    public async Task<IMongoCollection<BsonDocument>?> GetCollectionAsync(string name, string dbName)
    {
        var collection = await GetCollectionNamesAsync(dbName);
        if (collection == null)
            return null;
        if (!collection!.Contains(name))
            return null;
        //var items = await collection.Find(_ => true).ToListAsync();
        return Databases![dbName]?.GetCollection<BsonDocument>(name)!;
    }
    
    public List<object>? GetCollectionToJson(string name, string dbName)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return null;
        return MongoAssistant.CollectionToJson(collection).ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }
    public async Task<List<object>?> GetCollectionToJsonAsync(string name, string dbName)
    {
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return null;
        return MongoAssistant.CollectionToJson(collection).ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }

    // Read

    public BsonDocument? GetItem(string key, string value, string collectionName, string dbName)
    {
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(MongoAssistant.CreateFilter(key, value)).FirstOrDefault();
    }
    public async Task<BsonDocument?> GetItemAsync(string key, string value, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(MongoAssistant.CreateFilter(key, value)).FirstOrDefault();
    }
    public object? GetItemJson(string key, string value, string collectionName, string dbName)
    {
        var result = GetItem(key, value, collectionName, dbName);
        if (result == null)
            return null;
        MongoAssistant.FixId(result);
        return MongoAssistant.BsonMapper(result);
    }
    public async Task<object?> GetItemJsonAsync(string key, string value, string collectionName, string dbName)
    {
        var result = await GetItemAsync(key, value, collectionName, dbName);
        if (result == null)
            return null;
        MongoAssistant.FixId(result);
        return MongoAssistant.BsonMapper(result);
    }

    public BsonDocument? GetItem(string id, string collectionName, string dbName)
    {
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(MongoAssistant.CreateFilter(id)).FirstOrDefault();
    }
    public async Task<BsonDocument?> GetItemAsync(string id, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(MongoAssistant.CreateFilter(id)).FirstOrDefault();
    }
    public object? GetItemJson(string id, string collectionName, string dbName)
    {
        var result = GetItem(id, collectionName, dbName);
        if (result == null)
            return null;
        MongoAssistant.FixId(result);
        return MongoAssistant.BsonMapper(result);
    }
    public async Task<object?> GetItemJsonAsync(string id, string collectionName, string dbName)
    {
        var result = await GetItemAsync(id, collectionName, dbName);
        if (result == null)
            return null;
        MongoAssistant.FixId(result);
        return MongoAssistant.BsonMapper(result);
    }

    // Create

    public (string, object?) Add(string json, string name, string dbName)
    {
        BsonDocument? bDoc = MongoAssistant.StringToBson(json);
        if (bDoc == null)
            return (string.Empty, null);
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return (string.Empty, null);
        collection.InsertOne(bDoc);
        string id = MongoAssistant.FixId(bDoc);
        return (id, MongoAssistant.BsonMapper(bDoc));
    }
    public async Task<(string, object?)> AddAsync(string json, string name, string dbName)
    {
        BsonDocument? bDoc = MongoAssistant.StringToBson(json);
        if (bDoc == null)
            return (string.Empty, null);
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return (string.Empty, null);
        await collection.InsertOneAsync(bDoc);
        string id = MongoAssistant.FixId(bDoc);
        return (id, MongoAssistant.BsonMapper(bDoc));
    }

    // Update

    public bool Edit(string json, string id, string collectionName, string dbName)
    {
        BsonDocument? bDoc = MongoAssistant.StringToBson(json);
        if (bDoc == null)
            return false;
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return false;
        var result = collection.ReplaceOne(MongoAssistant.CreateFilter(id), bDoc);
        if (result.MatchedCount == 0)
            return false;
        else
            return true;
    }
    public async Task<bool> EditAsync(string json, string id, string collectionName, string dbName)
    {
        BsonDocument? inDoc = MongoAssistant.StringToBson(json);
        if (inDoc == null)
            return false;
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return false;
        var result = await collection.ReplaceOneAsync(MongoAssistant.CreateFilter(id), inDoc);
        if (result.MatchedCount == 0)
            return false;
        else
            return true;
    }

    // Delete

    public bool Remove(string id, string name, string dbName)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return false;
        var result = collection.DeleteOne(MongoAssistant.CreateFilter(id));
        if (result.DeletedCount == 0)
            return false;
        else
            return true;
    }
    public async Task<bool> RemoveAsync(string id, string name, string dbName)
    {
        return await RemoveAsync(MongoAssistant.CreateFilter(id), name, dbName);
    }
    public async Task<bool> RemoveAsync(List<string> ids, string name, string dbName)
    {
        return await RemoveAsync(MongoAssistant.CreateFilter(ids), name, dbName);
    }
    public async Task<bool> RemoveAsync(string propertyName, string propertyValue, string name, string dbName)
    {
        return await RemoveAsync(MongoAssistant.CreateFilter(propertyName, propertyValue), name, dbName);
    }
    private async Task<bool> RemoveAsync(FilterDefinition<BsonDocument> filter, string name, string dbName)
    {
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return false;
        //var result = await collection.DeleteOneAsync(IdFilter(id));
        var result = await collection.DeleteManyAsync(filter);
        if (result.DeletedCount == 0)
            return false;
        else
            return true;
    }

}