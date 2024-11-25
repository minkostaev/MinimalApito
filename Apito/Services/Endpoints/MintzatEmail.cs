namespace Apito.Services.Endpoints;

using System.Text.Json;

public static class MintzatEmail
{
    public static void Map(RouteGroupBuilder app)
    {
        app.MapPost("", PostOneAsync);
        
    }

    private static async Task<IResult> PostOneAsync(HttpContext context)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var bodyType = new { from = "", topic = "", message = "" };
        var obj = JsonSerializer.Deserialize(requestBody, bodyType.GetType());
        if (obj != null)
        {
            var from = ((dynamic)obj).from;
            var topic = ((dynamic)obj).topic;
            var message = ((dynamic)obj).message;
        }
        ;
        //var (id, itemJason) = await crud.AddAsync(requestBody, CollectionName!, DatabasesName!);
        //if (string.IsNullOrEmpty(id))
        //    return Results.NotFound();
        //return Results.Created($"/items/{id}", itemJason);
        return Results.Created();
    }
    

}