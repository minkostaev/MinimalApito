namespace Apito.Extensions;

using Apito.Models;
using Apito.Services;
using Mintzat.Email.ResendCom;
using MongoDB.Driver;

public static class BuilderServices
{
    public static async Task AddAll(this IServiceCollection services, ConfigurationManager configuration)
    {
        CustomLogger.Add("BuilderServices", CustomLogger.GetLine(), $"version: {AppValues.Version}");
        services.AddSwaggerServices();

        AppValues.Cors = configuration.GetSection("CORS:Allow-Origins").Get<string[]>()!;
        services.AddCors(options =>
        {
            options.AddPolicy(name: configuration["CORS:Policy-Name"]!,
            cnfg => { cnfg.WithOrigins(AppValues.Cors).AllowAnyMethod().AllowAnyHeader(); });
            ///cnfg => { cnfg.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });

        // Get secrets from my vault
        var vault = new VaultConfiguration(configuration["Vault"]!);
        string[] secretKeys = [configuration["DbMongo:kkkppp"]!, configuration["Emails:resend"]!];
        var connections = await vault.Get(secretKeys);

        if (connections != null)
        {
            if (connections.Count > 0)
                AppValues.MongoConnection = connections[0];
            if (connections.Count > 1)
                AppValues.ResendConnection = connections[1];
            CustomLogger.Add("BuilderServices", CustomLogger.GetLine(), $"connections count: {connections.Count}");
        }
        else
        {
            CustomLogger.Add("BuilderServices", CustomLogger.GetLine(), "connections failed");
        }

        if (!string.IsNullOrWhiteSpace(AppValues.MongoConnection))
        {
            services.AddSingleton<IMongoClient>(new MongoClient(AppValues.MongoConnection));
        }
        else
        {
            services.AddSingleton<IMongoClient>(new MongoClient());
            CustomLogger.Add("BuilderServices", CustomLogger.GetLine(), "Mongo Connection empty");
        }
        services.AddSingleton<MongoCrud>();
        if (!string.IsNullOrWhiteSpace(AppValues.ResendConnection))
        {
            services.AddSingleton<IResendSender>(new ResendSender(AppValues.ResendConnection));
        }
        else
        {
            CustomLogger.Add("BuilderServices", CustomLogger.GetLine(), "Resend Connection empty");
        }

    }

}