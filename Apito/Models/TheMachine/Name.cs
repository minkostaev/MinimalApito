namespace Apito.Models.TheMachine;

public class Name
{
    public Name()
    {
        User = Environment.UserName;
        Machine = Environment.MachineName;
        Domain = Environment.UserDomainName;
    }

    public string User { get; private set; }
    public string Machine { get; private set; }
    public string Domain { get; private set; }
}