namespace Apito.Services;

using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

public static class MongoAssistant
{
    //private static FilterDefinition<BsonDocument> IdFilter(string id) { return Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id)); }
    //private static FilterDefinition<BsonDocument> IdsFilter(List<string> ids) { return Builders<BsonDocument>.Filter.In("_id", ids.Select(id => new ObjectId(id))); }
    //private static FilterDefinition<BsonDocument> CustomFilter(string key, string value) { return Builders<BsonDocument>.Filter.Eq(key, value); }
    //private static BsonDocument? StringToBson(string json)
    //{
    //    BsonDocument doc;
    //    try { doc = BsonSerializer.Deserialize<BsonDocument>(json); }
    //    catch { return null; }
    //    return doc;
    //}
    //private static string? BsonToString(BsonDocument bDoc)
    //{
    //    try { return JsonSerializer.Serialize(bDoc); }
    //    catch { return null; }
    //}
    //private static string FixId(BsonDocument bDoc)
    //{
    //    var bDocId = bDoc["_id"];
    //    if (bDocId == null)
    //        return string.Empty;
    //    string result = bDocId.ToString()!;
    //    //bDoc["_id"] = bDoc["_id"].ToString();
    //    bDoc.Set("_id", result);
    //    return result;
    //}
    //private static object BsonMapper(BsonDocument bDoc) { return BsonTypeMapper.MapToDotNetValue(bDoc); }

}