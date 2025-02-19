namespace Apito.Models;

public static class AppSettings
{
    public static string[]? CorsOrigins { get; private set; }
    public static string? CorsName { get; private set; }
    public static string? Auth0Domain { get; private set; }
    public static string? Auth0Audience { get; private set; }
    public static string? Auth0Authority => $"https://{Auth0Domain}/";
    public static string? Vault { get; private set; }
    public static string? MongoKkkppp { get; private set; }
    public static string? EmailsResend { get; private set; }

    public static void Init(ConfigurationManager configuration)
    {
        CorsOrigins = configuration.GetSection("CORS:Allow-Origins").Get<string[]>();
        CorsName = configuration["CORS:Policy-Name"];
        Auth0Domain = configuration["Auth0:Domain"];
        Auth0Audience = configuration["Auth0:Audience"];
        Vault = configuration["Vault"];
        MongoKkkppp = configuration["DbMongo:kkkppp"];
        EmailsResend = configuration["Emails:resend"];
    }
}