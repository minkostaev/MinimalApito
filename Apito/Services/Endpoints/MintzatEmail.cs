namespace Apito.Services.Endpoints;

using Mintzat.Email.ResendCom;
using System.Text.Json;

public static class MintzatEmail
{
    public static void Map(RouteGroupBuilder app)
    {
        app.MapPost("", PostOneAsync);
        
    }

    private static async Task<IResult> PostOneAsync(HttpContext context, IResendSender sender)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var bodyType = new { to = "", from = "", name = "", topic = "", message = "" };
        var obj = JsonSerializer.Deserialize(requestBody, bodyType.GetType());

        string to = "";
        string from = "";
        string name = "";
        string topic = "";
        string message = "";

        if (obj != null)
        {
            to = ((dynamic)obj).to;
            from = ((dynamic)obj).from;
            name = ((dynamic)obj).name;
            topic = ((dynamic)obj).topic;
            message = ((dynamic)obj).message;
        }

        bool isSent = await SendEmail(sender, [to], from, name, topic, message);

        if (!isSent)
            return Results.NotFound();
        return Results.Created();
    }

    private static async Task<bool> SendEmail(IResendSender sender, string[] toEmails, string fromEmail, string name, string topic, string message)
    {
        string defaultMail = "minkostaev@yahoo.com";
        if (toEmails == null || string.IsNullOrEmpty(toEmails[0]))
            toEmails = [defaultMail];
        var result = await sender.SendEmail("no-reply@apito.somee.com",
            toEmails, topic, message, fromEmail,
            null, [defaultMail], null, name);
        return result.Item1;
    }


}