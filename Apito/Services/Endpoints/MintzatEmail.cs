namespace Apito.Services.Endpoints;

using Mintzat.Email.ResendCom;
using System.Text.Json;

public static class MintzatEmail
{
    public static void Map(RouteGroupBuilder app)
    {
        app.MapPost("", PostOneAsync).RequireAuthorization();
        
    }

    private static async Task<IResult> PostOneAsync(HttpContext context, IResendSender sender)
    {
        string requestBody = await HttpContextHelper.GetContextBodyAsync(context);
        var bodyType = new
        {
            to = "",
            from = "",
            name = "",
            topic = "",
            message = "",
            secondTopic = "",
            secondHeader = "",
            secondFooter = ""
        };
        var obj = JsonSerializer.Deserialize(requestBody, bodyType.GetType());

        string to = string.Empty;// optional
        string from = string.Empty;
        string name = string.Empty;
        string topic = string.Empty;
        string message = string.Empty;
        string secondTopic = string.Empty;// optional
        string secondHeader = string.Empty;// optional
        string secondFooter = string.Empty;// optional

        if (obj != null)
        {
            to = ((dynamic)obj).to;
            from = ((dynamic)obj).from;
            name = ((dynamic)obj).name;
            topic = ((dynamic)obj).topic;
            message = ((dynamic)obj).message;
            secondTopic = ((dynamic)obj).secondTopic;
            secondHeader = ((dynamic)obj).secondHeader;
            secondFooter = ((dynamic)obj).secondFooter;
        }

        bool isSent = await SendEmail(sender, [to], from, name, topic, message);
        if (!string.IsNullOrEmpty(secondTopic))
        {
            await SendEmail(sender, [from], to, "", secondTopic,
                secondHeader + topic + "<br/>" + message + secondFooter);
        }

        return ResponseResults.Post(isSent, null);
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