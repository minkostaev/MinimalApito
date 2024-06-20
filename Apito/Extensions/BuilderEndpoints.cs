namespace Apito.Extensions;

using Apito.Models;
using Apito.Services.Endpoints;
using Microsoft.AspNetCore.Builder;

public static class BuilderEndpoints
{
    public static void RegisterAllEndpoints(this IEndpointRouteBuilder app)
    {
        var items = app.MapGroup("/items");
        Items.Map(items, "Users", "ShortcutsGrid");

        var itemsAsync = app.MapGroup("/itemsasync");
        ItemsAsync.Map(itemsAsync, "Users", "ShortcutsGrid");

        //ApiVersionSet apiVersionSet = app.NewApiVersionSet()
        //    .HasApiVersion(new Asp.Versioning.ApiVersion(1))
        //    .HasApiVersion(new Asp.Versioning.ApiVersion(2))
        //    .ReportApiVersions()
        //    .Build();

        var imoti = app.MapGroup("/imoti");
        Imoti.Map(imoti, "Imoti", "ShortcutsGrid");

        var machinesDetails = app.MapGroup("/machinesdetails");//.WithApiVersionSet(apiVersionSet);
        MachinesDetails.Map(machinesDetails, "MachinesDetails", "ShortcutsGrid");

        var machinesLogs = app.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");
        
        var machinesRecords = app.MapGroup("/machinesrecords");
        MachinesRecords.Map(machinesRecords, "MachinesRecords", "ShortcutsGrid");

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