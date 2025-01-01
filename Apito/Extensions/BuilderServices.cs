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
            ///cnfg => { cnfg.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });

        ///string? vaultKey = Environment.GetEnvironmentVariable("Vault");

        // Get secrets from my vault
        CustomLogger.Add("BuilderServices", "25", configuration["Vault"]!);
        var vault = new VaultConfiguration(configuration["Vault"]!);
        string[] secretKeys = [configuration["DbMongo:kkkppp"]!, configuration["Emails:resend"]!];
        var connections = await vault.Get(secretKeys);

        if (connections != null)
        {
            if (connections.Count > 0)
                AppValues.MongoConnection = connections[0];
            if (connections.Count > 1)
                AppValues.ResendConnection = connections[1];
        }

        if (!string.IsNullOrWhiteSpace(AppValues.MongoConnection))
        {
            services.AddSingleton<IMongoClient>(new MongoClient(AppValues.MongoConnection));
        }
        else
        {
            services.AddSingleton<IMongoClient>(new MongoClient());
            AppValues.MongoFailed = true;
        }
        services.AddSingleton<MongoCrud>();
        if (!string.IsNullOrWhiteSpace(AppValues.ResendConnection))
        {
            services.AddSingleton<IResendSender>(new ResendSender(AppValues.ResendConnection));
        }

    }

}