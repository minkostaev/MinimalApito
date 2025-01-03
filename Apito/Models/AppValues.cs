namespace Apito.Models;

using System.Reflection;

public static class AppValues
{
                                  ///Assembly.GetAssembly(typeof(Program)).GetName().Version;
    public static string? Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    public static string? Name => Assembly.GetExecutingAssembly().GetName().Name;
    ///string? vaultKey = Environment.GetEnvironmentVariable("Vault");

    public static string MongoConnection { get; set; } = string.Empty;

    public static string ResendConnection { get; set; } = string.Empty;

    public static string[]? Cors { get; set; }

    public static List<string> DeployedPaths
    {
        get
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            var parent = Directory.GetParent(currentDirectory);
            string parentDirectory = parent!.FullName;
            List<string> result = [];
            result.Add("----Directories----");
            result.Add(currentDirectory);
            result.AddRange([.. Directory.GetDirectories(currentDirectory)]);
            result.Add(parentDirectory);
            result.AddRange([.. Directory.GetDirectories(parentDirectory)]);
            result.Add("----Files----");
            result.Add(Path.Combine(currentDirectory, "Resources", "Swagger.html"));
            result.AddRange(Directory.GetFiles(parentDirectory));
            foreach (var dir in Directory.GetDirectories(currentDirectory))
                result.AddRange(Directory.GetFiles(dir));
            return result;
        }
    }

}