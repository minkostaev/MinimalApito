namespace Apito.Services.Endpoints;

public class Imoti
{
    private static string? CollectionName { get; set; }
    private static string? DatabasesName { get; set; }

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

    private static async Task<IResult> GetAll(MongoCrud crud, HttpContext context)
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

    private static async Task<IResult> GetOne(MongoCrud crud, HttpContext context, string id)
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
        return await GetOne(crud, context, id);
    }

    private static async Task<IResult> DeleteOne(MongoCrud crud, HttpContext context, string id)
    {
        var deleted = await crud.RemoveAsync(id, CollectionName!, DatabasesName!);
        if (!deleted)
            return Results.NotFound();
        return Results.NoContent();
    }

}