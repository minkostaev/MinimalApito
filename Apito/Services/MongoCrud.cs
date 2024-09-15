namespace Apito.Services;

using MongoDB.Bson;
using MongoDB.Driver;

public class MongoCrud(IMongoClient client) : MongoInit(client)
{
    // READ
    public static BsonDocument? GetItem(FilterDefinition<BsonDocument>? filter, IMongoCollection<BsonDocument>? collection)
    {
        if (collection == null)
            return null;
        if (filter == null)
            return null;
        return collection.Find(filter).FirstOrDefault();
    }

    public static object? GetItemJson(BsonDocument? document)
    {
        if (document == null)
            return null;
        MongoAssistant.FixId(document);
        return MongoAssistant.BsonMapper(document);
    }

    public BsonDocument? GetItem(string id, string collectionName, string dbName)
    {
        var collection = GetCollection(collectionName, dbName);
        return GetItem(MongoAssistant.CreateFilter(id), collection);
    }
    public BsonDocument? GetItem(string key, string value, string collectionName, string dbName)
    {
        var collection = GetCollection(collectionName, dbName);
        return GetItem(MongoAssistant.CreateFilter(key, value), collection);
    }

    public async Task<BsonDocument?> GetItemAsync(string id, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        return GetItem(MongoAssistant.CreateFilter(id), collection);
    }
    public async Task<BsonDocument?> GetItemAsync(string key, string value, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        return GetItem(MongoAssistant.CreateFilter(key, value), collection);
    }

    public object? GetItemJson(string id, string collectionName, string dbName)
    {
        var result = GetItem(id, collectionName, dbName);
        return GetItemJson(result);
    }
    public object? GetItemJson(string key, string value, string collectionName, string dbName)
    {
        var result = GetItem(key, value, collectionName, dbName);
        return GetItemJson(result);
    }

    public async Task<object?> GetItemJsonAsync(string id, string collectionName, string dbName)
    {
        var result = await GetItemAsync(id, collectionName, dbName);
        return GetItemJson(result);
    }
    public async Task<object?> GetItemJsonAsync(string key, string value, string collectionName, string dbName)
    {
        var result = await GetItemAsync(key, value, collectionName, dbName);
        return GetItemJson(result);
    }

    // multi

    public async Task<List<BsonDocument>?> GetItemsAsync(string key, string value, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return null;
        var filter = MongoAssistant.CreateFilter(key, value);
        if (filter == null)
            return null;
        return collection.Find(filter).ToList();
    }
    public async Task<List<object>?> GetItemsJsonAsync(string key, string value, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return null;
        var filter = MongoAssistant.CreateFilter(key, value);
        if (filter == null)
            return null;
        var filteredList = MongoAssistant.CollectionToJson(collection, filter);
        return filteredList.ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }

    // CREATE

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

    // UPDATE

    public bool Edit(string json, string id, string collectionName, string dbName)
    {
        BsonDocument? bDoc = MongoAssistant.StringToBson(json);
        if (bDoc == null)
            return false;
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return false;
        var filter = MongoAssistant.CreateFilter(id);
        if (filter == null)
            return false;
        var result = collection.ReplaceOne(filter, bDoc);
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
        var filter = MongoAssistant.CreateFilter(id);
        if (filter == null)
            return false;
        var result = await collection.ReplaceOneAsync(filter, inDoc);
        if (result.MatchedCount == 0)
            return false;
        else
            return true;
    }

    // DELETE

    public bool Remove(string id, string name, string dbName)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return false;
        var filter = MongoAssistant.CreateFilter(id);
        if (filter == null)
            return false;
        var result = collection.DeleteOne(filter);
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
    private async Task<bool> RemoveAsync(FilterDefinition<BsonDocument>? filter, string name, string dbName)
    {
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return false;
        if (filter == null)
            return false;
        ///var result = await collection.DeleteOneAsync(IdFilter(id));
        var result = await collection.DeleteManyAsync(filter);
        if (result.DeletedCount == 0)
            return false;
        else
            return true;
    }

}
//https://mongodb.github.io/mongo-csharp-driver/2.5/reference/driver/crud/reading/
//https://mongodb.github.io/mongo-csharp-driver/2.5/reference/driver/crud/writing/
//https://chsakell.gitbook.io/mongodb-csharp-docs/crud-basics/update-documents/replace
//https://chsakell.gitbook.io/mongodb-csharp-docs/crud-basics/delete-documents