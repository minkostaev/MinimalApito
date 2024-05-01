﻿namespace Apito.Services.Endpoints;

using Apito.Models;
using Apito.Models.TheMachine;
using MongoDB.Bson.Serialization;
using System.Text.Json;

public static class MachinesDetails
{
    private static string? CollectionName { get; set; }
    private static string? DatabasesName { get; set; }

    public static void Map(RouteGroupBuilder app, string collectionName, string databasesName)
    {
        CollectionName = collectionName;
        DatabasesName = databasesName;

        app.MapGet("", GetAll);
        app.MapPost("", PostOne);

        //var item = app.MapGroup("/{id}");//id:guid

        //item.MapGet("", GetOne);
        //item.MapPut("", PutOne);
        //item.MapDelete("", DeleteOne);
    }

    private static async Task<IResult> GetAll(MongoCrud crud, HttpContext context)
    {
        var jsonList = await crud.GetCollectionToJsonAsync(CollectionName!, DatabasesName!);
        if (jsonList == null)
            return Results.NotFound();
        return Results.Ok(jsonList);
    }

    private static async Task<IResult> PostOne(MongoCrud crud, HttpContext context)
    {
        string machineHeader = context.Request.Headers["Desktop-Machine"]!;
        
        await MachinesLogs.PostOne(crud, context);

        var machineExist = await crud.GetItemJsonAsync("Hash", machineHeader, CollectionName!, DatabasesName!);
        if (machineExist != null)
            return Results.NotFound();

        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var (id, itemJason) = await crud.AddAsync(requestBody, CollectionName!, DatabasesName!);
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created($"/machinesdetails/{id}", itemJason);
    }

}