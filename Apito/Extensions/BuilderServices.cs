namespace Apito.Extensions;

using Apito.Models;
using Apito.Services;
using MongoDB.Driver;

public static class BuilderServices
{
    public static async Task AddAll(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSwaggerServices();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy(name: configuration["CORS:Policy-Name"]!,
            config =>
            {
                config.WithOrigins(configuration.GetSection("CORS:Allow-Origins").Get<string[]>()!)
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });

        // Get db connection string from my vault
        var vault = new VaultConfiguration(configuration["Vault"]!);
        var connection = await vault.Get(configuration["DbMongo:kkkppp"]!);
        ///configuration["DbMongoConnection"] = connection;

        // Mongo
        if (connection is string)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(connection.ToString()));
        }
        else
        {
            services.AddSingleton<IMongoClient>(new MongoClient());
            AppValues.MongoFailed = true;
        }
        services.AddSingleton<MongoCrud>();

    }

}