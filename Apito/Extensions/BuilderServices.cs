﻿namespace Apito.Extensions;

using Apito.Models;
using Apito.Services;
using Mintzat.Email.ResendCom;
using MongoDB.Driver;

public static class BuilderServices
{
    public static async Task AddAll(this IServiceCollection services)
    {
        CustomLogger.Add("BuilderServices", CustomLogger.GetLine(), $"version: {AppValues.Version}");

        services.AddSwaggerServices();//Extension

        //Auth0
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = AppValues.Bearer;
            options.DefaultChallengeScheme = AppValues.Bearer;
        }).AddJwtBearer(options =>
        {
            options.Authority = AppSettings.Auth0Authority;
            options.Audience = AppSettings.Auth0Audience;
        });
        services.AddAuthorization();

        services.AddCors(options =>
        {
            options.AddPolicy(name: AppSettings.CorsName!,
            cnfg => { cnfg.WithOrigins(AppSettings.CorsOrigins!).AllowAnyMethod().AllowAnyHeader(); });
            ///cnfg => { cnfg.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });

        // Get secrets from my vault
        var vault = new VaultConfiguration(AppSettings.Vault!);
        string[] secretKeys = [AppSettings.MongoKkkppp!, AppSettings.EmailsResend!];
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