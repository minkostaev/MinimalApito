namespace Apito.Services.Endpoints;

using System.Text.Json;

public static class MachinesDetails
{
    private static string CollectionName => "MachinesDetails";
    private static string DatabasesName => "ShortcutsGrid";
    private static string RootEndpoint => "/machinesdetails";

    public static void Map(IEndpointRouteBuilder root)
    {
        var app = root.MapGroup(RootEndpoint);

        app.MapGet("", GetAll);
        app.MapPost("", PostOne);
        app.MapGet("/hash/{hash}", GetByHash);

        var item = app.MapGroup("/{id}");//id:guid

        item.MapGet("", GetOne);
        //item.MapPut("", PutOne);
        item.MapDelete("", Delete);
    }

    private static async Task<IResult> GetAll(MongoCrud crud)
    {
        var jsonList = await crud.GetCollectionToJsonAsync(CollectionName, DatabasesName);
        if (jsonList == null)
            return Results.NotFound();
        return Results.Ok(jsonList);
    }

    private static async Task<IResult> GetOne(MongoCrud crud, string id)
    {
        var item = await crud.GetItemJsonAsync(id, CollectionName, DatabasesName);
        if (item == null)
            return Results.NotFound();
        return Results.Ok(item);
    }

    private static async Task<IResult> GetByHash(MongoCrud crud, string hash)
    {
        var items = await crud.GetItemsJsonAsync("Hash", hash, CollectionName, DatabasesName);
        if (items == null)
            return Results.NotFound();
        return Results.Ok(items);
    }

    private static async Task<IResult> PostOne(MongoCrud crud, HttpContext context)
    {
        string machineHeader = context.Request.Headers["Desktop-Machine"]!;
        ///if (machineHeader == null)
        ///    return Results.NotFound();

        await MachinesRecords.PostOne(crud, context);

        var machineExist = await crud.GetItemJsonAsync("Hash", machineHeader, CollectionName, DatabasesName);
        if (machineExist != null)
            return Results.NotFound();

        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var (id, itemJason) = await crud.AddAsync(requestBody, CollectionName, DatabasesName);
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created($"/machinesdetails/{id}", itemJason);
    }


    private static async Task<IResult> Delete(MongoCrud crud, HttpContext context, string id)
    {
        var item = await crud.GetItemJsonAsync(id, CollectionName, DatabasesName);
        var json = JsonSerializer.Serialize(item);
        var hashElement = MongoAssistant.JsonGetProperty(json, "Hash");
        if (hashElement != null)
        {
            string hash = hashElement?.GetString()!;
            await MachinesLogs.Delete(crud, context, "Hash", hash);
        }

        var deleted = await crud.RemoveAsync(id, CollectionName, DatabasesName);
        if (!deleted)
            return Results.NotFound();
        return Results.NoContent();
    }

}