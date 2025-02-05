namespace Apito.Extensions;

using Apito.Models;
using Apito.Services.Endpoints;
using Microsoft.AspNetCore.Builder;

public static class BuilderEndpoints
{
    public static void RegisterAllEndpoints(this IEndpointRouteBuilder root)
    {
        var items = root.MapGroup("/items");
        Items.Map(items, "Users", "ShortcutsGrid");

        var itemsAsync = root.MapGroup("/itemsasync");
        ItemsAsync.Map(itemsAsync, "Users", "ShortcutsGrid");

        ///ApiVersionSet apiVersionSet = root.NewApiVersionSet()
        ///    .HasApiVersion(new Asp.Versioning.ApiVersion(1))
        ///    .HasApiVersion(new Asp.Versioning.ApiVersion(2))
        ///    .ReportApiVersions()
        ///    .Build();

        var imoti = root.MapGroup("/imoti");
        Imoti.Map(imoti, "Imoti", "ShortcutsGrid");

        ///var machinesDetails = root.MapGroup("/machinesdetails");///.WithApiVersionSet(apiVersionSet);
        MachinesDetails.Map(root);

        var machinesLogs = root.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");
        
        MachinesRecords.Map(root);

        var emailResend = root.MapGroup("/emailresend");
        MintzatEmail.Map(emailResend);

        root.MapGet("/logger", () => { return CustomLogger.Get(); }).WithName("GetLogger").WithOpenApi();
        root.MapGet("/cors", () => { return AppValues.Cors; }).WithName("GetCors").WithOpenApi();
        root.MapGet("/paths", () => { return AppValues.DeployedPaths; }).WithName("GetPaths").WithOpenApi();
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