namespace Apito.Extensions;

using Apito.Models;
using Apito.Services.Endpoints;
using Microsoft.AspNetCore.Builder;
using SharpCompress.Common;

public static class BuilderEndpoints
{
    public static void RegisterAllEndpoints(this IEndpointRouteBuilder app)
    {
        var items = app.MapGroup("/items");
        Items.Map(items, "Users", "ShortcutsGrid");

        var itemsAsync = app.MapGroup("/itemsasync");
        ItemsAsync.Map(itemsAsync, "Users", "ShortcutsGrid");

        ///ApiVersionSet apiVersionSet = app.NewApiVersionSet()
        ///    .HasApiVersion(new Asp.Versioning.ApiVersion(1))
        ///    .HasApiVersion(new Asp.Versioning.ApiVersion(2))
        ///    .ReportApiVersions()
        ///    .Build();

        var imoti = app.MapGroup("/imoti");
        Imoti.Map(imoti, "Imoti", "ShortcutsGrid");

        var machinesDetails = app.MapGroup("/machinesdetails");///.WithApiVersionSet(apiVersionSet);
        MachinesDetails.Map(machinesDetails, "MachinesDetails", "ShortcutsGrid");

        var machinesLogs = app.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");
        
        var machinesRecords = app.MapGroup("/machinesrecords");
        MachinesRecords.Map(machinesRecords, "MachinesRecords", "ShortcutsGrid");

        var emailResend = app.MapGroup("/emailresend");
        MintzatEmail.Map(emailResend);

        app.MapGet("/version", () => { return AppValues.Version; }).WithName("GetVersion").WithOpenApi();
        app.MapGet("/paths", () =>
        {
            List<string> resList = Directory.GetDirectories(Directory.GetCurrentDirectory()).ToList();
            resList.Add(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Swagger.html"));
            resList.Add(Directory.GetCurrentDirectory());
            var parent = Directory.GetParent(Directory.GetCurrentDirectory());
            resList.Add(parent!.FullName);
            resList.AddRange([.. Directory.GetDirectories(parent!.FullName)]);
            resList.Add("----Files----");
            resList.AddRange(Directory.GetFiles(parent!.FullName));
            foreach (var dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                resList.AddRange(Directory.GetFiles(dir));
            return resList;
        }).WithName("GetPaths").WithOpenApi();
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