namespace Apito.Services;

public static class ResponseResults
{

    public static IResult Post(string id, string path, object? item)
    {
        if (string.IsNullOrEmpty(id))
            return Results.NotFound();
        return Results.Created(path, item);
    }

    public static IResult Get(object? obj)
    {
        if (obj == null)
            return Results.NotFound();
        return Results.Ok(obj);
    }

    ///public static IResult Put(bool success)
    ///{
    ///    if (!success)
    ///        return Results.NotFound();
    ///    return Results.Ok();
    ///}

    public static IResult Delete(bool success)
    {
        if (!success)
            return Results.NotFound();
        return Results.NoContent();
    }

}