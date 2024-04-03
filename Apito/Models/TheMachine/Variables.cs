namespace Apito.Models.TheMachine;

using System.Collections;

public class Variables
{
    public Variables()
    {
        CurrentDirectory = Environment.CurrentDirectory;
        FileExecuted = Environment.ProcessPath;//CommandLine
        UserVariables = [];
        try { AddUserVariables(); }
        catch (Exception) { }
    }
    public void AddUserVariables()
    {
        int errors = 0;
        foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables())
        {
            if (variable.Key != null && variable.Value != null)
            {
                try { UserVariables.Add(variable.Key.ToString()!, variable.Value.ToString()); }
                catch (Exception)
                {
                    errors++;
                    if (UserVariables.ContainsKey("ERRORS"))
                    {
                        UserVariables["ERRORS"] = errors.ToString();
                    }
                    else
                    {
                        UserVariables.Add("ERRORS", errors.ToString());
                    }
                }
            }
        }
    }

    public string CurrentDirectory { get; private set; }
    public string? FileExecuted { get; private set; }
    public Dictionary<string, string?> UserVariables { get; private set; }
}