namespace Apito.Models;

public static class AppSettings
{
    public static string? CorsName { get; private set; }
    public static string[]? CorsOrigins { get; private set; }
    public static string? Auth0Domain { get; private set; }
    public static string? Auth0Audience { get; private set; }
    public static string? Auth0Authority => $"https://{Auth0Domain}/";
    public static string? Vault { get; private set; }
    public static string? MongoKkkppp { get; private set; }
    public static string? EmailsResend { get; private set; }
    public static List<SwaggerInfo>? Swaggers { get; private set; }

    public static void Init(ConfigurationManager configuration)
    {
        CorsName = configuration["CORS:Policy-Name"];
        CorsOrigins = configuration.GetSection("CORS:Allow-Origins").Get<string[]>();
        Auth0Domain = configuration["Auth0:Domain"];
        Auth0Audience = configuration["Auth0:Audience"];
        Vault = configuration["Vault"];
        MongoKkkppp = configuration["DbMongo:kkkppp"];
        EmailsResend = configuration["Emails:resend"];
        Swaggers = [];
        int i = 0;
        bool hasSwaggers = true;
        while (hasSwaggers)
        {
            i++;
            var swaggerSection = configuration.GetSection($"Swagger:v{i}");
            hasSwaggers = swaggerSection.Exists();
            var swaggerObj = swaggerSection.Get<SwaggerInfo>();
            if (swaggerObj != null)
                Swaggers.Add(swaggerObj);
        }
    }
}