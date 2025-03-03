namespace Apito.Extensions;

using Apito.Models;

public static class BuilderApp
{
    public static void Init(this WebApplicationBuilder builder)//1
    {
        AppValues.IsDevelopment = builder.Environment.IsDevelopment();
        AppSettings.Init(builder.Configuration);//appsettings.json to objects
    }

    public static void AddAllUses(this WebApplication app)//4
    {
        ///AppValues.IsDevelopment = app.Environment.IsDevelopment();
        app.UseCors(AppSettings.CorsName!);
        app.UseHttpsRedirection();

        //Auth0
        app.UseAuthentication();
        app.UseAuthorization();

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

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Version", AppValues.Version);
            await next();
        });

    }

}