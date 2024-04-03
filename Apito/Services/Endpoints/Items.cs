namespace Apito.Services.Endpoints;

using System.Text;

public static class Items
{
    public static void GetPost(RouteGroupBuilder app)
    {
        app.MapGet("", GetAll);
        app.MapPost("", PostOne);
    }
    public static void GetPutDelete(RouteGroupBuilder app)
    {
        app.MapGet("", GetOne);
        app.MapPut("", PutOne);
        app.MapDelete("", DeleteOne);
    }

    private static IResult GetAll(MongoCrud crud, HttpContext context)
    {
        //string customHeaderValue = context.Request.Headers["Xaaa"]!;

        var jsonList = crud.GetCollectionToJson("Users", "ShortcutsGrid");
        if (jsonList == null)
        {
            return Results.NotFound();
            //context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        //await context.Response.WriteAsJsonAsync(jsonList);
        return Results.Ok(jsonList);
    }

    private static async Task<IResult> PostOne(MongoCrud crud, HttpContext context)
    {
        //string customHeaderValue = context.Request.Headers["Xaaa"]!;

        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        string requestBody = await reader.ReadToEndAsync();
        reader.Dispose();
        var (id, itemJason) = await crud.AddAsync("Users", "ShortcutsGrid", requestBody);
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        //await context.Response.WriteAsJsonAsync(item);
        return Results.Created($"/items/{id}", itemJason);
    }

    private static IResult GetOne(MongoCrud crud, HttpContext context, string id)
    {
        //string customHeaderValue = context.Request.Headers["Xaaa"]!;

        var item = crud.GetItemJson(id, "Users", "ShortcutsGrid");
        if (item == null)
            return Results.NotFound();
        //context.Response.StatusCode = StatusCodes.Status404NotFound;
        return Results.Ok(item);
        //await context.Response.WriteAsJsonAsync(item);
    }

    private static async Task<IResult> PutOne(MongoCrud crud, HttpContext context, string id)
    {
        //string customHeaderValue = context.Request.Headers["Xaaa"]!;

        string requestBody;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        bool edited = await crud.EditAsync(id, "Users", "ShortcutsGrid", requestBody);
        if (!edited)
        {
            //context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Results.NotFound();
        }
        return GetOne(crud, context, id);
    }

    private static async Task<IResult> DeleteOne(MongoCrud crud, HttpContext context, string id)
    {
        //string customHeaderValue = context.Request.Headers["Xaaa"]!;

        var deleted = await crud.RemoveAsync(id, "Users", "ShortcutsGrid");
        if (!deleted)
        {
            //context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Results.NotFound();
        }
        //context.Response.StatusCode = StatusCodes.Status204NoContent;
        return Results.NoContent();
    }

}