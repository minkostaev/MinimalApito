namespace Apito.Extensions;

using Apito.Services.Endpoints;

public static class EndpointRouteBuilder
{
    public static void RegisterAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/weatherforecast", () =>
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                )).ToArray();
            return forecast;
        }).WithName("GetWeatherForecast").WithOpenApi();

        var items = app.MapGroup("/items");
        Items.Map(items, "Users", "ShortcutsGrid");

        var itemsAsync = app.MapGroup("/itemsasync");
        ItemsAsync.Map(itemsAsync, "Users", "ShortcutsGrid");

        var machinesDetails = app.MapGroup("/machinesdetails");
        MachinesDetails.Map(machinesDetails, "MachinesDetails", "ShortcutsGrid");

        var machinesLogs = app.MapGroup("/machineslogs");
        MachinesLogs.Map(machinesLogs, "MachinesLogs", "ShortcutsGrid");
    }

}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}