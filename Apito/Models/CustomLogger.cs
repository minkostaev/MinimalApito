namespace Apito.Models;

using System.Runtime.CompilerServices;

public static class CustomLogger
{
    private static readonly List<string> _logs = [];
    public static List<string> Get() => _logs;
    public static void Add(object thisFile, int line, string message)
    {
        Add(thisFile.GetType().Name, line, message);
        ///var name = typeof(CustomLogger).Name;
    }
    public static void Add(string file, int line, string message)
    {
        _logs.Add($"{file}__{line}__{message}");
    }
    public static int GetLine([CallerLineNumber] int lineNumber = 0) => lineNumber;
}