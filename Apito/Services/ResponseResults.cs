namespace Apito.Services;

public static class ResponseResults
{

    public static IResult Post(bool success, object? item, string error = "")
    {
        if (!success)
            return Results.BadRequest(error);
        return Results.Ok(item);
    }

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
// 1) Ok: Returns a 200 OK response with an optional value.
///  Results.Ok(value);
// 2) NotFound: Returns a 404 Not Found response.
///  Results.NotFound();
// 3) BadRequest: Returns a 400 Bad Request response with an optional error message.
///  Results.BadRequest("Error message");
// 4) Created: Returns a 201 Created response, typically used when a resource is successfully created.
///  Results.Created(location, value);
// 5) NoContent: Returns a 204 No Content response.
///  Results.NoContent();
// 6) Unauthorized: Returns a 401 Unauthorized response.
///  Results.Unauthorized();
// 7) Forbid: Returns a 403 Forbidden response.
///  Results.Forbid();
// 8) Conflict: Returns a 409 Conflict response.
///  Results.Conflict("Conflict message");
// 9) Redirect: Returns a 302 Found response, redirecting the client to a different URL.
///  Results.Redirect(url);