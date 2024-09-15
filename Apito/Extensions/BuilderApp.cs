namespace Apito.Extensions;

using Apito.Models;

public static class BuilderApp
{
    public static void AddAllUses(this WebApplication app)
    {
        app.UseStaticFiles();// wwwroot
        
        app.UseCors(app.Configuration["CORS:Policy-Name"]!);
        app.UseHttpsRedirection();

        ///if (app.Environment.IsDevelopment())
        ///{
        app.AddSwaggerUses();
        ///}

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Version", AppValues.Version);
            await next();
        });

    }

}