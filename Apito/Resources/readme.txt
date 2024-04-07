Custom swagger:

1) Add the html template as Embedded resource

2) Use the html as stream like this:
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "v1");
    string embeddedHtml = $"{AppValues.Name}.Resources.Swagger.html";
    c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedHtml);
});
