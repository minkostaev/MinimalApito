namespace Apito.Extensions;

using Apito.Models;
using Microsoft.AspNetCore.Routing;

public static class BuilderMethods
{
    //public static void AddEndpoints(this IEndpointRouteBuilder app)
    //{

    //}

    public static void AddAllUses(this WebApplication app)
    {
        app.UseCors(app.Configuration["CORS:Policy-Name"]!);
        app.UseHttpsRedirection();
        
        // Swagger
        //if (app.Environment.IsDevelopment())
        //{
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "{documentName}/swagger.{json|yaml}";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "v1");
            c.RoutePrefix = "";
        });
        //}

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Version", AppValues.Version);
            await next();
        });

    }

}