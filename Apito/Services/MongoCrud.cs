namespace Apito.Services;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

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
    private BsonDocument? StringToBson(string document)
    {
        BsonDocument doc;
        try { doc = BsonSerializer.Deserialize<BsonDocument>(document); }
        catch { return null; }
        return doc;
    }

    private string FixId(BsonDocument doc)
    {
        var bDocId = doc["_id"];
        if (bDocId == null)
            return string.Empty;
        string result = bDocId.ToString()!;
        //doc["_id"] = doc["_id"].ToString();
        doc.Set("_id", result);
        return result;
    }

    // PUBLIC

    public List<string>? GetCollectionNames(string dbName)
    {
        if (!GetDatabase(dbName))
            return null;
        return Databases![dbName]?.ListCollectionNames().ToList();
    }
    public IMongoCollection<BsonDocument>? GetCollection(string name, string dbName)
    {
        var collection = GetCollectionNames(dbName);
        if (collection == null)
            return null;
        if (!collection!.Contains(name))
            return null;

        //var collection = db.GetCollection<BsonDocument>("Users");
        //var items = await collection.Find(_ => true).ToListAsync();

        return Databases![dbName]?.GetCollection<BsonDocument>(name)!;
    }
    public List<object>? GetCollectionToJson(string name, string dbName)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return null;
        
        //var result = await collection.Find(new BsonDocument()).ToListAsync();
        //var obj = result.ToJson();

        var mongoList = new List<BsonDocument>();
        using (var cursor = collection.Find(new BsonDocument()).ToCursor())
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
        return mongoList.ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }

    // Read

    public BsonDocument? GetItem(string id, string name, string dbName)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return null;
        return collection.Find(IdFilter(id)).FirstOrDefault();
    }
    public object? GetItemJson(string id, string name, string dbName)
    {
        var result = GetItem(id, name, dbName);
        if (result == null)
            return null;
        FixId(result);
        return BsonTypeMapper.MapToDotNetValue(result);
    }

    // Create

    //public BsonDocument? Add(string name, string dbName, string document)
    //{
    //    var collection = GetCollection(name, dbName);
    //    if (collection == null)
    //        return null;
    //    var bsonDoc = StringToBson(document);
    //    collection.InsertOne(bsonDoc);
    //    return bsonDoc;
    //}
    public async Task<(string, object?)> AddAsync(string name, string dbName, string document)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return (string.Empty, null);
        var bsonDoc = StringToBson(document);
        if (bsonDoc == null)
            return (string.Empty, null);
        await collection.InsertOneAsync(bsonDoc);
        string id = FixId(bsonDoc);
        return (id, BsonTypeMapper.MapToDotNetValue(bsonDoc));
    }

    // Update

    //public bool Edit(string id, string name, string dbName, string document)
    //{
    //    BsonDocument? inDoc = StringToBson(document);
    //    if (inDoc == null)
    //        return false;
    //    var collection = GetCollection(name, dbName);
    //    if (collection == null)
    //        return false;
    //    var result = collection.ReplaceOne(IdFilter(id), inDoc);
    //    if (result.MatchedCount == 0)
    //        return false;
    //    else
    //        return true;
    //}
    public async Task<bool> EditAsync(string id, string name, string dbName, string document)
    {
        BsonDocument? inDoc = StringToBson(document);
        if (inDoc == null)
            return false;
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return false;
        var result = await collection.ReplaceOneAsync(IdFilter(id), inDoc);
        if (result.MatchedCount == 0)
            return false;
        else
            return true;
    }

    // Delete

    //public bool Remove(string id, string name, string dbName)
    //{
    //    var collection = GetCollection(name, dbName);
    //    if (collection == null)
    //        return false;
    //    var result = collection.DeleteOne(IdFilter(id));
    //    if (result.DeletedCount == 0)
    //        return false;
    //    else
    //        return true;
    //}
    public async Task<bool> RemoveAsync(string id, string name, string dbName)
    {
        var collection = GetCollection(name, dbName);
        if (collection == null)
            return false;
        var result = await collection.DeleteOneAsync(IdFilter(id));
        if (result.DeletedCount == 0)
            return false;
        else
            return true;
    }

    //    // Object to JSON
    //    string jsonString = JsonSerializer.Serialize(document);
    //    // JSON string to BsonDocument
    //    return BsonSerializer.Deserialize<BsonDocument>(jsonString);

}