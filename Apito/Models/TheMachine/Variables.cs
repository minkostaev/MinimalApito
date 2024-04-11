namespace Apito.Models.TheMachine;

public class Variables
{
    public string? CurrentDirectory { get; set; }
    public string? FileExecuted { get; set; }
    public Dictionary<string, string?>? UserVariables { get; set; }
}