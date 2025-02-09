﻿namespace Apito.Extensions;

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
        Imoti.Map(imoti, "Imoti", "ShortcutsGrid");

        MachinesDetails.Map(root);

        var machinesLogs = root.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");
        
        MachinesRecords.Map(root);

        var emailResend = root.MapGroup("/emailresend");
        MintzatEmail.Map(emailResend);

        root.MapGet("/logger", () => { return CustomLogger.Get(); }).WithName("GetLogger").WithOpenApi();
        root.MapGet("/cors", () => { return AppValues.Cors; }).WithName("GetCors").WithOpenApi();
        root.MapGet("/paths", () => { return AppValues.DeployedPaths; }).WithName("GetPaths").WithOpenApi();
        root.MapGet("/auth", [Authorize] () => "This is a secure endpoint!");
        //root.MapGet("/auth", () => "This is a secure endpoint!").RequireAuthorization();
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