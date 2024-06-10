using Apito.Models;
using System.Text.Json;

namespace Apito.Services.Endpoints;

public static class MachinesRecords
{
    private static string? CollectionName { get; set; }
    private static string? DatabasesName { get; set; }

    public static void Map(RouteGroupBuilder app, string collectionName, string databasesName)
    {
        CollectionName = collectionName;
        DatabasesName = databasesName;

        app.MapGet("", GetAll);
        app.MapPost("", PostOne);
        app.MapDelete("", DeleteMany);

        //var item = app.MapGroup("/{id}");//id:guid

        //item.MapGet("", GetOne);
        //item.MapPut("", PutOne);
        //item.MapDelete("", DeleteOne).AddMore(
        //    "DeleteLog",
        //    "DeleteLog",
        //    "DeleteLog");
    }


    private static async Task<IResult> GetAll(MongoCrud crud, HttpContext context)
    {
        var jsonList = await crud.GetCollectionToJsonAsync(CollectionName!, DatabasesName!);
        if (jsonList == null)
            return Results.NotFound();
        return Results.Ok(jsonList);
    }

    private static async Task<IResult> DeleteMany(MongoCrud crud, HttpContext context)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var result = MongoAssistant.GetIdsFromJson(requestBody);
        var deleted = await crud.RemoveAsync(result, CollectionName!, DatabasesName!);
        if (!deleted)
            return Results.NotFound();
        return Results.NoContent();
    }

    public static async Task<IResult> PostOne(MongoCrud crud, HttpContext context)
    {
        string machineHeader = context.Request.Headers["Desktop-Machine"]!;
        string valueHeader = context.Request.Headers["Desktop-Value"]!;
        string versionHeader = context.Request.Headers["Desktop-Version"]!;

        var record = new
        {
            Hash = machineHeader,
            Value = valueHeader,
            Client = versionHeader,
            Server = $"Apito|{AppValues.Version}",
            Date = DateTime.Now
        };

        string recordJson = JsonSerializer.Serialize(record);// new
        var (id, itemJason) = await crud.AddAsync(recordJson, "MachinesRecords", DatabasesName!);// new
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created($"/machineslogs/{id}", itemJason);
    }


}