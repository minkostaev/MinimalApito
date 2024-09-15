namespace Apito.Services;

using MongoDB.Bson;
using MongoDB.Driver;

public class MongoInit
{
    private readonly IMongoClient mongoClient;
    public MongoInit(IMongoClient client)
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

    public List<string>? GetDatabaseNames
    {
        get
        {
            if (Databases == null)
                return null;
            var list = new List<string>();
            foreach (var dbName in Databases.Keys)
            {
                list.Add(dbName);
            }
            return list;
        }
    }

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
        var cursorList = await Databases![dbName]!.ListCollectionNamesAsync();
        return await cursorList.ToListAsync();
    }

    public IMongoCollection<BsonDocument>? GetCollection(string name, string dbName)
    {
        var collection = GetCollectionNames(dbName);
        if (collection == null)
            return null;
        if (!collection!.Contains(name))
            return null;
        ///var items = await collection.Find(_ => true).ToListAsync();
        return Databases![dbName]?.GetCollection<BsonDocument>(name)!;
    }
    public async Task<IMongoCollection<BsonDocument>?> GetCollectionAsync(string name, string dbName)
    {
        var collection = await GetCollectionNamesAsync(dbName);
        if (collection == null)
            return null;
        if (!collection!.Contains(name))
            return null;
        ///var items = await collection.Find(_ => true).ToListAsync();
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

}