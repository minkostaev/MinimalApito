﻿namespace Apito.Services.Endpoints;

using Apito.Models;
using System.Text.Json;

public static class MachinesRecords
{
    private static string CollectionName => "MachinesRecords";
    private static string DatabasesName => "ShortcutsGrid";

    public static void Map(RouteGroupBuilder app)
    {
        ///var aaa = app.MapGroup("/machinesrecords");

        app.MapGet("", GetAll);
        app.MapPost("", PostOne);
        app.MapDelete("", DeleteMany);

        var item = app.MapGroup("/{id}");//id:guid

        item.MapGet("", GetOne);
        item.MapDelete("", DeleteOne);
        ///item.MapPut("", PutOne);
        ///item.MapDelete("", DeleteOne).AddMore(
        ///    "DeleteLog",
        ///    "DeleteLog",
        ///    "DeleteLog");
    }

    private static async Task<IResult> GetAll(MongoCrud crud)
    {
        var jsonList = await crud.GetCollectionToJsonAsync(CollectionName!, DatabasesName!);
        return ResponseResults.Get(jsonList);
    }
    public static async Task<IResult> PostOne(MongoCrud crud, HttpContext context)
    {
        string machineHeader = context.Request.Headers["Desktop-Machine"]!;
        string valueHeader = context.Request.Headers["Desktop-Value"]!;
        string versionHeader = context.Request.Headers["Desktop-Version"]!;
        string timeHeader = context.Request.Headers["Desktop-Time"]!;

        var record = new
        {
            Hash = machineHeader,
            Value = valueHeader,
            Client = versionHeader,
            Server = $"Apito|{AppValues.Version}",
            Date = DateTime.Now,
            Time = timeHeader
        };

        string recordJson = JsonSerializer.Serialize(record);
        var (id, itemJason) = await crud.AddAsync(recordJson, CollectionName!, DatabasesName!);
        return ResponseResults.Post(id, $"/machinesrecords/{id}", itemJason);
    }
    private static async Task<IResult> DeleteMany(MongoCrud crud, HttpContext context)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var result = MongoAssistant.GetIdsFromJson(requestBody);
        var deleted = await crud.RemoveAsync(result, CollectionName!, DatabasesName!);
        return ResponseResults.Delete(deleted);
    }

    private static async Task<IResult> GetOne(MongoCrud crud, string id)
    {
        ///?page=2&size=10&sort=price,asc&filter=category:electronics

        ///NameValueCollection queryParams = HttpUtility.ParseQueryString(id);
        ///int page = int.Parse(queryParams["page"]);
        ///int size = int.Parse(queryParams["size"]);
        ///string sort = queryParams["sort"];
        ///string filter = queryParams["filter"];

        var item = await crud.GetItemJsonAsync(id, CollectionName, DatabasesName);
        return ResponseResults.Get(item);
    }
    private static async Task<IResult> DeleteOne(MongoCrud crud, string id)
    {
        var deleted = await crud.RemoveAsync(id, CollectionName, DatabasesName);
        return ResponseResults.Delete(deleted);
    }

}