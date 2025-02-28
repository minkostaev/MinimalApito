namespace Apito.Models;

using Microsoft.OpenApi.Models;

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
        try
        {
            Uri? termsOfService = null;
            if (!string.IsNullOrWhiteSpace(swaggerInfo.TermsOfService))
                termsOfService = new Uri(swaggerInfo.TermsOfService);
            Uri? licenseUrl = null;
            if (!string.IsNullOrWhiteSpace(swaggerInfo.LicenseUrl))
                licenseUrl = new Uri(swaggerInfo.LicenseUrl);
            Uri? contactUrl = null;
            if (!string.IsNullOrWhiteSpace(swaggerInfo.ContactUrl))
                contactUrl = new Uri(swaggerInfo.ContactUrl);
            return new OpenApiInfo
            {
                Version = AppValues.Version,
                Title = swaggerInfo.Title,
                Description = swaggerInfo.Description,
                TermsOfService = termsOfService,
                License = new OpenApiLicense
                {
                    Name = swaggerInfo.LicenseName,
                    Url = licenseUrl
                },
                Contact = new OpenApiContact
                {
                    Name = swaggerInfo.ContactName,
                    Email = swaggerInfo.ContactEmail,
                    Url = contactUrl
                }
            };
        }
        catch (Exception) { return new OpenApiInfo(); }
    }

}