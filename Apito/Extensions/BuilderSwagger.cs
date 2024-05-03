namespace Apito.Extensions;

using Apito.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

public static class BuilderSwagger
{
    public static void AddSwaggerServices(this IServiceCollection services)
    {
        // Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        
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
                    Url = new Uri("https://github.com/minkostaev/MinimalApito")
                },
                License = new OpenApiLicense
                {
                    Name = "Source",
                    Url = new Uri("https://github.com/minkostaev/MinimalApito")
                }
            });

            options.SwaggerDoc("v2", new OpenApiInfo { Title = "Your API V2", Version = "v2" });


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
            c.SwaggerEndpoint("v2/swagger.json", "v2 Label");
            c.DocExpansion(DocExpansion.None);
            c.EnableTryItOutByDefault();
            string embeddedHtml = $"{AppValues.Name}.Resources.Swagger.html";
            c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedHtml);
        });
    }

}