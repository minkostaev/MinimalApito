namespace Apito.Services;

using System.Text;

public static class HttpContextHelper
{
    public static string GetContextBody(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        string requestBody = reader.ReadToEnd();
        reader.Dispose();
        //string requestBody;
        //using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        //{ requestBody = reader.ReadToEnd(); }
        return requestBody;
    }
    public static async Task<string> GetContextBodyAsync(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
        string requestBody = await reader.ReadToEndAsync();
        reader.Dispose();
        //string requestBody;
        //using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        //{ requestBody = reader.ReadToEnd(); }
        return requestBody;
    }

    //string customHeaderValue = context.Request.Headers["Xaaa"]!;
    //await context.Response.WriteAsJsonAsync(item);

}