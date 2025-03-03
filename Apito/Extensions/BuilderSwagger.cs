namespace Apito.Extensions;

using Apito.Models;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

public static class BuilderSwagger
{
    public static void AddSwaggerServices(this IServiceCollection services)//3
    {
        // Versioning
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            ///options.ApiVersionReader = new UrlSegmentApiVersionReader();
            ///options.ApiVersionReader = new QueryStringApiVersionReader("version");
            ///options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            options.ApiVersionReader = ApiVersionReader.Combine(
                //new QueryStringApiVersionReader(),
                new HeaderApiVersionReader("X-Version")
            );
        }).AddApiExplorer(options =>//Swagger
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        ///services.AddSwaggerGen();
        services.AddSwaggerGen(options =>
        {
            if (AppSettings.Swaggers != null)
            {
                foreach (var info in AppSettings.Swaggers)//add versions
                {
                    options.SwaggerDoc(info.Name, SwaggerInfo.CreateInfo(info));
                }
            }
            ///var versions = services.BuildServiceProvider()
            ///                       .GetRequiredService<IApiVersionDescriptionProvider>();
            ///foreach (var description in versions.ApiVersionDescriptions)
            ///{
            ///    options.SwaggerDoc(description.GroupName, new OpenApiInfo()
            ///    {
            ///        Title = $"My API {description.ApiVersion}",
            ///        Version = description.ApiVersion.ToString()
            ///    });
            ///}

            //Auth0
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = AppValues.Bearer,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = AppValues.Bearer
                }
            };
            options.AddSecurityDefinition(AppValues.Bearer, securitySchema);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            { { securitySchema, [AppValues.Bearer] } });
        });
    }

    public static void AddSwaggerUses(this WebApplication app)//6
    {
        //Properties/launchSettings.json
        string address = "swagger";//launchUrl
        app.UseSwagger(options =>
        {
            options.RouteTemplate = address + "/{documentName}/swagger.{json|yaml}";
        });
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = address;
            if (AppSettings.Swaggers != null)
            {
                foreach (var info in AppSettings.Swaggers)//add versions
                {
                    options.SwaggerEndpoint($"{info.Name}/swagger.json", info.Title);
                }
            }
            ///var versions = app.DescribeApiVersions();
            ///foreach (var description in versions)
            ///{
            ///    var url = $"/swagger/{description.GroupName}/swagger.json";
            ///    var name = description.GroupName.ToUpperInvariant();
            ///    options.SwaggerEndpoint(url, name);
            ///}
            options.DocExpansion(DocExpansion.None);
            options.EnableTryItOutByDefault();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "swagger", "swagger.html");
            if (File.Exists(filePath))
                options.IndexStream = () => new FileStream(filePath, FileMode.Open, FileAccess.Read);
        });
    }
}