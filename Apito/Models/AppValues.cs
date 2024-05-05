namespace Apito.Models;

using System.Reflection;

public static class AppValues
{
    public static string? Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                                   //Assembly.GetAssembly(typeof(Program)).GetName().Version;
    public static string? Name => Assembly.GetExecutingAssembly().GetName().Name;

    public static bool MongoFailed { get; set; }

}