namespace Apito.Extensions;

using Apito.Models;
using Apito.Services;
using Mintzat.Email.ResendCom;
using MongoDB.Driver;

public static class BuilderServices
{
    public static async Task AddAll(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSwaggerServices();

        AppValues.Cors = configuration.GetSection("CORS:Allow-Origins").Get<string[]>()!;
        services.AddCors(options =>
        {
            options.AddPolicy(name: configuration["CORS:Policy-Name"]!,
            cnfg => { cnfg.WithOrigins(AppValues.Cors).AllowAnyMethod().AllowAnyHeader(); });
        });

        ///string? vaultKey = Environment.GetEnvironmentVariable("Vault");

        // Get secrets from my vault
        var vault = new VaultConfiguration(configuration["Vault"]!);
        string[] secretKeys = [configuration["DbMongo:kkkppp"]!, configuration["Emails:resend"]!];
        var connections = await vault.Get(secretKeys);

        if (connections != null && connections.Count > 0)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(connections[0]));
        }
        else
        {
            services.AddSingleton<IMongoClient>(new MongoClient());
            AppValues.MongoFailed = true;
        }
        services.AddSingleton<MongoCrud>();
        if (connections != null && connections.Count > 1)
        {
            services.AddSingleton<IResendSender>(new ResendSender(connections[1]));
        }

    }

}