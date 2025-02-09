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
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = AppValues.Version,
                Title = "Apito",
                Description = "Apito Description todo",
                //TermsOfService = new Uri("https://github.com/minkostaev/MinimalApito"),
                Contact = new OpenApiContact
                {
                    Name = "NAME",
                    Email = "name@mail.com",
                    Url = new Uri($"https://github.com/minkostaev/MinimalApito")
                },
                License = new OpenApiLicense
                {
                    Name = "Source",
                    Url = new Uri($"https://github.com/minkostaev/MinimalApito")
                }
            });

            ////options.SwaggerDoc("v2", new OpenApiInfo { Title = "Your API V2", Version = "v2" });

            // Auth0
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            options.AddSecurityDefinition("Bearer", securitySchema);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            { { securitySchema, ["Bearer"] } });


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
        app.UseSwagger(c =>
        {
            c.RouteTemplate = address + "/{documentName}/swagger.{json|yaml}";
        });
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = address;
            c.SwaggerEndpoint("v1/swagger.json", "v1 Name");
            //c.SwaggerEndpoint("v2/swagger.json", "v2 Label");
            c.DocExpansion(DocExpansion.None);
            c.EnableTryItOutByDefault();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "swagger", "swagger.html");
            if (File.Exists(filePath))
                c.IndexStream = () => new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            //foreach (var description in provider.ApiVersionDescriptions)
            //{
            //    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            //                            description.GroupName.ToUpperInvariant());
            //}

        });
    }

}