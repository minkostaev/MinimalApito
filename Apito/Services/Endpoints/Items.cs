namespace Apito.Services.Endpoints;

using System.Collections;
using System.Text;

public static class Items
{
    public static void Map(RouteGroupBuilder app, string collectionName, string databasesName)
    {
        CollectionName = collectionName;
        DatabasesName = databasesName;

        app.MapGet("", GetAll);
        app.MapPost("", PostOne);

        var item = app.MapGroup("/{id}");//id:guid

        item.MapGet("", GetOne);
        item.MapPut("", PutOne);
        item.MapDelete("", DeleteOne);
    }

    private static string CollectionName { get; set; }
    private static string DatabasesName { get; set; }

    private static IResult GetAll(MongoCrud crud)//, HttpContext context)
    {
        var jsonList = crud.GetCollectionToJson("Users", "ShortcutsGrid");
        if (jsonList == null)
            return Results.NotFound();
        return Results.Ok(jsonList);
    }

    private static IResult PostOne(MongoCrud crud, HttpContext context)
    {
        string requestBody = HttpContextHelper.GetContextBody(context);
        var (id, itemJson) = crud.Add(requestBody, CollectionName, DatabasesName);
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created($"/items/{id}", itemJson);
    }

    private static IResult GetOne(MongoCrud crud, HttpContext context, string id)
    {
        var item = crud.GetItemJson(id, CollectionName, DatabasesName);
        if (item == null)
            return Results.NotFound();
        return Results.Ok(item);
    }

    private static IResult PutOne(MongoCrud crud, HttpContext context, string id)
    {
        string requestBody = HttpContextHelper.GetContextBody(context);
        bool edited = crud.Edit(requestBody, id, CollectionName, DatabasesName);
        if (!edited)
            return Results.NotFound();
        return GetOne(crud, context, id);
    }

    private static IResult DeleteOne(MongoCrud crud, HttpContext context, string id)
    {
        var deleted = crud.Remove(id, CollectionName, DatabasesName);
        if (!deleted)
            return Results.NotFound();
        return Results.NoContent();
    }

}