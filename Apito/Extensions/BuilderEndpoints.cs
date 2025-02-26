namespace Apito.Extensions;

using Apito.Models;
using Apito.Services.Endpoints;
using Microsoft.AspNetCore.Authorization;
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

        var imoti = root.MapGroup("/imoti");///.WithApiVersionSet(apiVersionSet);
        Imoti.Map(imoti);

        MachinesDetails.Map(root);

        var machinesLogs = root.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");
        
        MachinesRecords.Map(root);

        var emailResend = root.MapGroup("/emailresend");
        MintzatEmail.Map(emailResend);

        root.MapGet("/logger", [Authorize] () =>
        { return CustomLogger.Get(); }).WithName("GetLogger").WithOpenApi();
        root.MapGet("/cors", [Authorize] () =>
        { return AppSettings.CorsOrigins; }).WithName("GetCors").WithOpenApi();
        root.MapGet("/paths", [Authorize] () => 
        { return AppValues.DeployedPaths; }).WithName("GetPaths").WithOpenApi();
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