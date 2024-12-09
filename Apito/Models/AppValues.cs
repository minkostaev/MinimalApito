﻿namespace Apito.Models;

using System.Reflection;

public static class AppValues
{
    public static string? Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                                   //Assembly.GetAssembly(typeof(Program)).GetName().Version;
    public static string? Name => Assembly.GetExecutingAssembly().GetName().Name;

    public static string? MongoConnection { get; set; }
    public static bool MongoFailed { get; set; }

    public static string? ResendConnection { get; set; }

    public static string[]? Cors { get; set; }

}