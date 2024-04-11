namespace Apito.Models.TheMachine;

using MongoDB.Bson;

public class Log
{
    //public ObjectId _id { get; set; }
    public string Hash { get; set; }
    public string Value { get; set; }
    public DateTime Date { get; set; }
}