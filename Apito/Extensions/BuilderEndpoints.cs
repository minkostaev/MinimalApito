namespace Apito.Extensions;

using Apito.Models;
using Apito.Services.Endpoints;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

public static class BuilderEndpoints
{
    public static void RegisterAllEndpoints(this IEndpointRouteBuilder root)//5
    {
        // Versioning
        var versionBuilder = root.NewApiVersionSet();
        int mjr = 0;
        if (AppSettings.Swaggers != null)
        {
            foreach (var info in AppSettings.Swaggers)//add versions
            {
                mjr++;
                versionBuilder.HasApiVersion(new ApiVersion(mjr));
            }
        }
        var versionSet = versionBuilder.ReportApiVersions().Build();
        ///var versionSet = root.NewApiVersionSet()
        ///    .HasApiVersion(new ApiVersion(1, 0))
        ///    .HasApiVersion(new ApiVersion(2, 0))
        ///    //.HasDeprecatedApiVersion(new ApiVersion(1.0))
        ///    //.HasApiVersion(new ApiVersion(2.0))
        ///    .ReportApiVersions().Build();

        var items = root.MapGroup("/items");
        Items.Map(items, "Users", "ShortcutsGrid");

        var itemsAsync = root.MapGroup("/itemsasync");
        ItemsAsync.Map(itemsAsync, "Users", "ShortcutsGrid");

        var imoti = root.MapGroup("/imoti");
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

        // Versioning examples
        root.MapGet("ver", () => "Example v1")
            .WithApiVersionSet(versionSet).MapToApiVersion(1.0);
        root.MapGet("ver", (HttpContext context) =>
        {
            var apiVersion = context.GetRequestedApiVersion();
            return "Version " + apiVersion?.MajorVersion?.ToString();
        }).WithApiVersionSet(versionSet).MapToApiVersion(2.0);

        // Versioning in the pattern
        root.MapGet("v{version:apiVersion}/wthr", (HttpContext context) =>
        {
            var apiVer = context.GetRequestedApiVersion();
            return "Version " + apiVer?.MajorVersion?.ToString();
        });

        root.MapGet("/pdf", () =>
        {
            var htmlContent = String.Format("<body>Hello world: {0}</body>", DateTime.Now);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            var byteResult = htmlToPdf.GeneratePdf(htmlContent);
            return Results.File(byteResult, "application/pdf", "pdf.pdf");
        }).WithName("GetPdf").WithOpenApi();
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