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
        var bodyType = new { to = "", from = "", name = "", topic = "", message = "", toUserToo = false };
        var obj = JsonSerializer.Deserialize(requestBody, bodyType.GetType());

        string to = string.Empty;
        string from = string.Empty;
        string name = string.Empty;
        string topic = string.Empty;
        string message = string.Empty;
        bool toUserToo = false;
        ///string secondTopic = string.Empty;
        ///string secondHeader = string.Empty;
        ///string secondFooter = string.Empty;

        if (obj != null)
        {
            to = ((dynamic)obj).to;
            from = ((dynamic)obj).from;
            name = ((dynamic)obj).name;
            topic = ((dynamic)obj).topic;
            message = ((dynamic)obj).message;
            toUserToo = ((dynamic)obj).toUserToo;
        }

        bool isSent = await SendEmail(sender, [to], from, name, topic, message);
        if (toUserToo)
        {
            await SendEmail(sender, [from], to, "",
                "Summary and confirmation on message you sent",
                "<h1>You have send this message to us</h1>" +
                topic + "<br/>" + message +
                "<h2>We'll get back to you as soon as possible</h2>");
        }

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