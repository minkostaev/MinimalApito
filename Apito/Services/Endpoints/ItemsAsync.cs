namespace Apito.Services.Endpoints;

public class ItemsAsync
{
    private static string? CollectionName { get; set; }
    private static string? DatabasesName { get; set; }

    public static void Map(RouteGroupBuilder app, string collectionName, string databasesName)
    {
        CollectionName = collectionName;
        DatabasesName = databasesName;

        app.MapGet("", GetAllAsync);
        app.MapPost("", PostOneAsync);

        var item = app.MapGroup("/{id}");//id:guid

        item.MapGet("", GetOneAsync);
        item.MapPut("", PutOneAsync);
        item.MapDelete("", DeleteOneAsync);
    }

    private static async Task<IResult> GetAllAsync(MongoCrud crud)///, HttpContext context)
    {
        var jsonList = await crud.GetCollectionToJsonAsync(CollectionName!, DatabasesName!);
        if (jsonList == null)
            return Results.NotFound();
        return Results.Ok(jsonList);
    }

    private static async Task<IResult> PostOneAsync(MongoCrud crud, HttpContext context)
    {
        string requestBody = HttpContextHelper.GetContextBody(context);
        var (id, itemJason) = await crud.AddAsync(requestBody, CollectionName!, DatabasesName!);
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created($"/items/{id}", itemJason);
    }

    private static async Task<IResult> GetOneAsync(MongoCrud crud, HttpContext context, string id)
    {
        var item = await crud.GetItemJsonAsync(id, CollectionName!, DatabasesName!);
        if (item == null)
            return Results.NotFound();
        return Results.Ok(item);
    }

    private static async Task<IResult> PutOneAsync(MongoCrud crud, HttpContext context, string id)
    {
        string requestBody = HttpContextHelper.GetContextBody(context);
        bool edited = await crud.EditAsync(requestBody, id, CollectionName!, DatabasesName!);
        if (!edited)
            return Results.NotFound();
        return await GetOneAsync(crud, context, id);
    }

    private static async Task<IResult> DeleteOneAsync(MongoCrud crud, HttpContext context, string id)
    {
        var deleted = await crud.RemoveAsync(id, CollectionName!, DatabasesName!);
        if (!deleted)
            return Results.NotFound();
        return Results.NoContent();
    }

}