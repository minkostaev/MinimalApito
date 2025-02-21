namespace Apito.Extensions;

using Apito.Models;

public static class BuilderApp
{
    public static void AddAllUses(this WebApplication app)
    {
        app.UseCors(AppSettings.CorsName!);
        app.UseHttpsRedirection();

        app.UseDefaultFiles();// This will look for index.html by default
        app.UseStaticFiles();// wwwroot
        ///app.Use(async (context, next) =>
        ///{
        ///    if (context.Request.Path == "/")
        ///    {
        ///        context.Response.Redirect("/index.html");
        ///        return;
        ///    }
        ///    await next();
        ///});

        app.AddSwaggerUses();//Extension

        app.UseAuthentication();//Auth0
        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Version", AppValues.Version);
            await next();
        });

    }

}