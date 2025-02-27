using Microsoft.OpenApi.Models;

namespace Apito.Models;

public class SwaggerInfo
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? TermsOfService { get; set; }
    public string? LicenseUrl { get; set; }
    public string? LicenseName { get; set; }
    public string? ContactUrl { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }

    public static OpenApiInfo CreateInfo(SwaggerInfo swaggerInfo)
    {
        return new OpenApiInfo
        {
            Version = AppValues.Version,
            Title = swaggerInfo.Title,
            Description = swaggerInfo.Description,
            TermsOfService = new Uri(swaggerInfo.TermsOfService),
            License = new OpenApiLicense
            {
                Name = swaggerInfo.LicenseName,
                Url = new Uri(swaggerInfo.LicenseUrl)
            },
            Contact = new OpenApiContact
            {
                Name = swaggerInfo.ContactName,
                Email = swaggerInfo.ContactEmail,
                Url = new Uri(swaggerInfo.ContactUrl)
            }
        };
    }

}