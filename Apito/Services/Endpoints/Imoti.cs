﻿namespace Apito.Services.Endpoints;

public static class Imoti
{
    private static string CollectionName => "Imoti";
    private static string DatabasesName => "ShortcutsGrid";
    ///private static string RootEndpoint => "/imoti";

    public static void Map(RouteGroupBuilder app)
    {
        app.MapGet("", GetAll).RequireAuthorization();
        app.MapPost("", PostOne);
        
        var item = app.MapGroup("/{id}").RequireAuthorization();//id:guid

        item.MapGet("", GetOne);
        item.MapPut("", PutOne);
        item.MapDelete("", DeleteOne);
    }

    private static async Task<IResult> GetAll(MongoCrud crud)
    {
        var jsonList = await crud.GetCollectionToJsonAsync(CollectionName!, DatabasesName!);
        if (jsonList == null)
            return Results.NotFound();
        return Results.Ok(jsonList);
    }
    private static async Task<IResult> PostOne(MongoCrud crud, HttpContext context)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var (id, itemJason) = await crud.AddAsync(requestBody, CollectionName!, DatabasesName!);
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created($"/imoti/{id}", itemJason);
    }

    private static async Task<IResult> GetOne(MongoCrud crud, string id)
    {
        var item = await crud.GetItemJsonAsync(id, CollectionName!, DatabasesName!);
        if (item == null)
            return Results.NotFound();
        return Results.Ok(item);
    }
    private static async Task<IResult> PutOne(MongoCrud crud, HttpContext context, string id)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        bool edited = await crud.EditAsync(requestBody, id, CollectionName!, DatabasesName!);
        if (!edited)
            return Results.NotFound();
        return await GetOne(crud, id);
    }
    private static async Task<IResult> DeleteOne(MongoCrud crud, string id)
    {
        var deleted = await crud.RemoveAsync(id, CollectionName!, DatabasesName!);
        if (!deleted)
            return Results.NotFound();
        return Results.NoContent();
    }

}