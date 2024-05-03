namespace Apito.Extensions;

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
        var vault = new VaultConfiguration(configuration);
        var connection = await vault.Get(configuration["DbMongo:kkkppp"]!);
        //configuration["DbMongoConnection"] = connection;

        // Mongo

        //var mongoClient = new MongoClient(connection);
        //var database1 = mongoClient.GetDatabase("ShortcutsGrid");
        //var database2 = mongoClient.GetDatabase("wildlife");
        //builder.Services.AddSingleton(database1);
        //builder.Services.AddSingleton(database2);
        if (connection is string)
            services.AddSingleton<IMongoClient>(new MongoClient(connection.ToString()));
        else
            services.AddSingleton<IMongoClient>(new MongoClient());
        services.AddSingleton<MongoCrud>();
        //builder.Services.AddSingleton<IMongoDatabase>(provider =>
        //{
        //    var mongoClient = provider.GetRequiredService<IMongoClient>();
        //    var dbName = mongoClient.ListDatabaseNames().FirstOrDefault();//ShortcutsGrid
        //    return mongoClient.GetDatabase(dbName);
        //});

    }

}