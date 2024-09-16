namespace Apito.Services;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;

public static class MongoAssistant
{
    public static FilterDefinition<BsonDocument>? CreateFilter(string id)
    {
        try { return Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id)); }
        catch (Exception) { return null; }
    }
    public static FilterDefinition<BsonDocument>? CreateFilter(List<string> ids)
    {
        try { return Builders<BsonDocument>.Filter.In("_id", ids.Select(id => new ObjectId(id))); }
        catch (Exception) { return null; }
    }
    public static FilterDefinition<BsonDocument>? CreateFilter(string key, string value)
    {
        try { return Builders<BsonDocument>.Filter.Eq(key, value); }
        catch (Exception) { return null; }
    }

    public static string FixDocId(BsonDocument bDoc)
    {
        var bDocId = bDoc["_id"];
        if (bDocId == null)
            return string.Empty;
        string result = bDocId.ToString()!;
        ///bDoc["_id"] = result;
        bDoc.Set("_id", result);
        return result;
    }
    public static object BsonMapper(BsonDocument bDoc)
    {
        return BsonTypeMapper.MapToDotNetValue(bDoc);
    }
    public static List<object> BsonMapper(List<BsonDocument> bDocs)
    {
        return bDocs.ConvertAll(BsonTypeMapper.MapToDotNetValue);
    }
    public static List<BsonDocument> CollectionToJson(IMongoCollection<BsonDocument> mongoCollection, FilterDefinition<BsonDocument>? filter = null)
    {
        ///var result = await collection.Find(new BsonDocument()).ToListAsync();
        ///var obj = result.ToJson();

        var mongoList = new List<BsonDocument>();
        filter ??= new BsonDocument();
        using (var cursor = mongoCollection.Find(filter).ToCursor())
        {
            while (cursor.MoveNext())
            {
                foreach (var doc in cursor.Current)
                {
                    FixDocId(doc);
                    mongoList.Add(doc);
                }
            }
        }
        return mongoList;
    }

    // ---

    public static BsonDocument? StringToBson(string json)
    {
        BsonDocument doc;
        try { doc = BsonSerializer.Deserialize<BsonDocument>(json); }
        catch { return null; }
        return doc;
    }
    public static string? BsonToString(BsonDocument bDoc)
    {
        try { return JsonSerializer.Serialize(bDoc); }
        catch { return null; }
    }

    // ---

    public static JsonElement? JsonGetProperty(string json, string name)
    {
        try
        {
            JsonDocument document = JsonDocument.Parse(json);
            JsonElement rootElement = document.RootElement;
            return rootElement.GetProperty(name);
        }
        catch { return null; }
    }
    public static List<string> GetIdsFromJson(string json)
    {
        var result = new List<string>();
        var idsElement = JsonGetProperty(json, "Ids");
        if (idsElement == null)
            return result;
        var eslement = ((JsonElement)idsElement).EnumerateArray();
        foreach (JsonElement idElement in eslement)
        {
            result.Add(idElement.GetString()!);
        }
        return result;
    }

}