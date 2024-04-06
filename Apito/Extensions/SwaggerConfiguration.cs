namespace Apito.Extensions;

using Apito.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

public static class SwaggerConfiguration
{
    public static void AddSwaggerUses(this WebApplication app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "{documentName}/swagger.{json|yaml}";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "v1");
            c.RoutePrefix = "";
            c.DocExpansion(DocExpansion.None);
            c.EnableTryItOutByDefault();
            string embeddedHtml = $"{AppValues.Name}.Resources.swagger.html";
            c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedHtml);
        });
        
        //app.UseSwaggerUI(c =>
        //{
        //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        //    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        //    c.RoutePrefix = string.Empty;
        //    c.EnableTryItOutByDefault();
        //    c.IndexStream = () => GetType().Assembly.GetManifestResourceStream($"{GetType().Namespace}.Resources.swagger.html");
        //    //c.IndexStream = () => app.GetType().GetTypeInfo().Assembly.GetManifestResourceStream($"{app.GetType().Namespace}.Resources.swagger.html");
        //});
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        // Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen();

        //var apiVersion = Assembly.GetAssembly(typeof(Program)).GetName().Version;

        //    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        //    //c.IncludeXmlComments(xmlPath);

        //});


        services.AddSwaggerGen(options =>
        {
            //options.ExampleFilters();

            //var apiVersion = Assembly.GetAssembly(typeof(Program)).GetName().Version;
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

}