﻿namespace Apito.Extensions;

using Apito.Models;
using Apito.Services.Endpoints;

public static class BuilderEndpoints
{
    public static void RegisterAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/weatherforecast", () =>
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                )).ToArray();
            return forecast;
        }).WithName("GetWeatherForecast").WithOpenApi();
        
        var items = app.MapGroup("/items");
        Items.Map(items, "Users", "ShortcutsGrid");

        var itemsAsync = app.MapGroup("/itemsasync");
        ItemsAsync.Map(itemsAsync, "Users", "ShortcutsGrid");

        //ApiVersionSet apiVersionSet = app.NewApiVersionSet()
        //    .HasApiVersion(new Asp.Versioning.ApiVersion(1))
        //    .HasApiVersion(new Asp.Versioning.ApiVersion(2))
        //    .ReportApiVersions()
        //    .Build();

        var machinesDetails = app.MapGroup("/machinesdetails");//.WithApiVersionSet(apiVersionSet);
        MachinesDetails.Map(machinesDetails, "MachinesDetails", "ShortcutsGrid");

        var machinesLogs = app.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");

        app.MapGet("/version", () => { return AppValues.Version; }).WithName("GetVersion").WithOpenApi();
    }

    public static void AddMore(this RouteHandlerBuilder routeHandlerBuilder,
        string name, string summary, string description)
    {
        routeHandlerBuilder.WithName(name)
            .WithOpenApi()
            .WithSummary(summary)
            .WithDescription(description);
    }

}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

//https://www.youtube.com/watch?v=9MOpm5id2AI&list=PLgRlicSxjeMOUGRV28LGyqDgL0IySmGJ6&index=3