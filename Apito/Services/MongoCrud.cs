namespace Apito.Services;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;

public class MongoCrud
{
    private readonly IMongoClient mongoClient;
    public MongoCrud(IMongoClient client)
    {
        mongoClient = client;
        Databases = [];
        foreach (var dbName in mongoClient.ListDatabaseNames().ToEnumerable())
        {
            IMongoDatabase? db = null;
            Databases?.Add(dbName, db);
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
    
    private FilterDefinition<BsonDocument> IdFilter(string id) { return Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id)); }
    private FilterDefinition<BsonDocument> IdsFilter(List<string> ids) { return Builders<BsonDocument>.Filter.In("_id", ids.Select(id => new ObjectId(id))); }
    private FilterDefinition<BsonDocument> CustomFilter(string key, string value) { return Builders<BsonDocument>.Filter.Eq(key, value); }
    private BsonDocument? StringToBson(string json)
    {
        BsonDocument doc;
        try { doc = BsonSerializer.Deserialize<BsonDocument>(json); }
        catch { return null; }
        return doc;
    }
    private string? BsonToString(BsonDocument bDoc)
    {
        try { return JsonSerializer.Serialize(bDoc); }
        catch { return null; }
    }
    private string FixId(BsonDocument bDoc)
    {
        var bDocId = bDoc["_id"];
        if (bDocId == null)
            return string.Empty;
        string result = bDocId.ToString()!;
        //bDoc["_id"] = bDoc["_id"].ToString();
        bDoc.Set("_id", result);
        return result;
    }
    private object BsonMapper(BsonDocument bDoc) { return BsonTypeMapper.MapToDotNetValue(bDoc); }
    private List<BsonDocument> CollectionToJson(IMongoCollection<BsonDocument> mongoCollection, FilterDefinition<BsonDocument>? filter = null)
    {
        //var result = await collection.Find(new BsonDocument()).ToListAsync();
        //var obj = result.ToJson();

        var mongoList = new List<BsonDocument>();
        filter ??= new BsonDocument();
        using (var cursor = mongoCollection.Find(filter).ToCursor())
        {
            while (cursor.MoveNext())
            {
                foreach (var doc in cursor.Current)
                {
                    FixId(doc);
                    mongoList.Add(doc);
                }
            }
        }
        return mongoList;
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
        return CollectionToJson(collection).ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }
    public async Task<List<object>?> GetCollectionToJsonAsync(string name, string dbName)
    {
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return null;
        return CollectionToJson(collection).ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }

    // Read

    public BsonDocument? GetItem(string key, string value, string collectionName, string dbName)
    {
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(CustomFilter(key, value)).FirstOrDefault();
    }
    public async Task<BsonDocument?> GetItemAsync(string key, string value, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(CustomFilter(key, value)).FirstOrDefault();
    }
    public object? GetItemJson(string key, string value, string collectionName, string dbName)
    {
        var result = GetItem(key, value, collectionName, dbName);
        if (result == null)
            return null;
        FixId(result);
        return BsonMapper(result);
    }
    public async Task<object?> GetItemJsonAsync(string key, string value, string collectionName, string dbName)
    {
        var result = await GetItemAsync(key, value, collectionName, dbName);
        if (result == null)
            return null;
        FixId(result);
        return BsonMapper(result);
    }

    public BsonDocument? GetItem(string id, string collectionName, string dbName)
    {
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(IdFilter(id)).FirstOrDefault();
    }
    public async Task<BsonDocument?> GetItemAsync(string id, string collectionName, string dbName)
    {
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return null;
        return collection.Find(IdFilter(id)).FirstOrDefault();
    }
    public object? GetItemJson(string id, string collectionName, string dbName)
    {
        var result = GetItem(id, collectionName, dbName);
        if (result == null)
            return null;
        FixId(result);
        return BsonMapper(result);
    }
    public async Task<object?> GetItemJsonAsync(string id, string collectionName, string dbName)
    {
        var result = await GetItemAsync(id, collectionName, dbName);
        if (result == null)
            return null;
        FixId(result);
        return BsonMapper(result);
    }




    public async Task<List<object>?> GetCollectionToJsonAsync(string propertyName, string propertyValue, string name, string dbName)
    {
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return null;
        return CollectionToJson(collection, CustomFilter(propertyName, propertyValue)).ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }

    // Create

    public (string, object?) Add(string json, string name, string dbName)
    {
        BsonDocument? bDoc = StringToBson(json);
        if (bDoc == null)
            return (string.Empty, null);
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return (string.Empty, null);
        collection.InsertOne(bDoc);
        string id = FixId(bDoc);
        return (id, BsonMapper(bDoc));
    }
    public async Task<(string, object?)> AddAsync(string json, string name, string dbName)
    {
        BsonDocument? bDoc = StringToBson(json);
        if (bDoc == null)
            return (string.Empty, null);
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return (string.Empty, null);
        await collection.InsertOneAsync(bDoc);
        string id = FixId(bDoc);
        return (id, BsonMapper(bDoc));
    }

    // Update

    public bool Edit(string json, string id, string collectionName, string dbName)
    {
        BsonDocument? bDoc = StringToBson(json);
        if (bDoc == null)
            return false;
        var collection = GetCollection(collectionName, dbName);
        if (collection == null)
            return false;
        var result = collection.ReplaceOne(IdFilter(id), bDoc);
        if (result.MatchedCount == 0)
            return false;
        else
            return true;
    }
    public async Task<bool> EditAsync(string json, string id, string collectionName, string dbName)
    {
        BsonDocument? inDoc = StringToBson(json);
        if (inDoc == null)
            return false;
        var collection = await GetCollectionAsync(collectionName, dbName);
        if (collection == null)
            return false;
        var result = await collection.ReplaceOneAsync(IdFilter(id), inDoc);
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
        var result = collection.DeleteOne(IdFilter(id));
        if (result.DeletedCount == 0)
            return false;
        else
            return true;
    }
    public async Task<bool> RemoveAsync(string id, string name, string dbName)
    {
        //var collection = await GetCollectionAsync(name, dbName);
        //if (collection == null)
        //    return false;
        //var result = await collection.DeleteOneAsync(IdFilter(id));
        //if (result.DeletedCount == 0)
        //    return false;
        //else
        //    return true;
        return await RemoveAsync(IdFilter(id), name, dbName);
    }
    public async Task<bool> RemoveAsync(List<string> ids, string name, string dbName)
    {
        //var collection = await GetCollectionAsync(name, dbName);
        //if (collection == null)
        //    return false;
        //var result = await collection.DeleteManyAsync(IdsFilter(ids));
        //if (result.DeletedCount == 0)
        //    return false;
        //else
        //    return true;
        return await RemoveAsync(IdsFilter(ids), name, dbName);
    }
    public async Task<bool> RemoveAsync(string propertyName, string propertyValue, string name, string dbName)
    {
        return await RemoveAsync(CustomFilter(propertyName, propertyValue), name, dbName);
    }
    private async Task<bool> RemoveAsync(FilterDefinition<BsonDocument> filter, string name, string dbName)
    {
        var collection = await GetCollectionAsync(name, dbName);
        if (collection == null)
            return false;
        var result = await collection.DeleteManyAsync(filter);
        if (result.DeletedCount == 0)
            return false;
        else
            return true;
    }

}