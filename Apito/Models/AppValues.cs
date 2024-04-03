namespace Apito.Models;

using System.Reflection;

public static class AppValues
{
    public static string? Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString();

}