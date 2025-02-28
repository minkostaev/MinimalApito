namespace Apito.Extensions;

using Apito.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

// required nugets:
//Microsoft.AspNetCore.OpenApi
//Swashbuckle.AspNetCore

public static class BuilderSwagger
{
    public static void AddSwaggerServices(this IServiceCollection services)
    {
        // Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        //// Add API versioning
        //services.AddApiVersioning(options =>
        //{
        //    options.AssumeDefaultVersionWhenUnspecified = true;
        //    options.DefaultApiVersion = new ApiVersion(1, 0);
        //    options.ReportApiVersions = true;
        //    options.ApiVersionReader = ApiVersionReader.Combine(
        //        new HeaderApiVersionReader("x-api-version")
        //    );
        //}).AddApiExplorer(options =>
        //{
        //    options.GroupNameFormat = "'v'VVV";
        //    options.SubstituteApiVersionInUrl = true;
        //});

        //services.AddSwaggerGen();
        services.AddSwaggerGen(options =>
        {
            //options.ExampleFilters();

            if (AppSettings.Swaggers != null)
            {
                foreach (var info in AppSettings.Swaggers)//add versions
                {
                    options.SwaggerDoc(info.Name, SwaggerInfo.CreateInfo(info));
                }
            }

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


            //var provider = services.BuildServiceProvider()
            //.GetRequiredService<IApiVersionDescriptionProvider>();

            //foreach (var description in provider.ApiVersionDescriptions)
            //{
            //    options.SwaggerDoc(description.GroupName, new OpenApiInfo()
            //    {
            //        Title = $"My API {description.ApiVersion}",
            //        Version = description.ApiVersion.ToString()
            //    });
            //}



            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //options.IncludeXmlComments(xmlPath);
            //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    In = ParameterLocation.Header,
            //    Description = "Please enter token",
            //    Name = "Authorization",
            //    Type = SecuritySchemeType.Http,
            //    BearerFormat = "JWT",
            //    Scheme = "bearer"
            //});
            //options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //     {
            //         {
            //             new OpenApiSecurityScheme
            //             {
            //                 Reference = new OpenApiReference
            //                 {
            //                     Type=ReferenceType.SecurityScheme,
            //                     Id="Bearer"
            //                 }
            //             },
            //             new string[]{}
            //         }
            //     });

            //options.OperationFilter<AddHeadersOperationFilter>();
            //options.OperationFilter<AnalyzeOperationsFilter>();

        });
        //services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
    }

    public static void AddSwaggerUses(this WebApplication app)
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
            options.DocExpansion(DocExpansion.None);
            options.EnableTryItOutByDefault();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "swagger", "swagger.html");
            if (File.Exists(filePath))
                options.IndexStream = () => new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            //foreach (var description in provider.ApiVersionDescriptions)
            //{
            //    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            //                            description.GroupName.ToUpperInvariant());
            //}

        });
    }

}