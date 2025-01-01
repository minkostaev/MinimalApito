namespace Apito.Models;

public static class CustomLogger
{
    private static readonly List<string> _logs = [];
    public static List<string> Get() => _logs;
    public static void Add(string file, string line, string message)
    {
        _logs.Add($"{file}__{line}__{message}");
    }
}