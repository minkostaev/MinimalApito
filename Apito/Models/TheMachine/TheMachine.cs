namespace Apito.Models.TheMachine;

public class TheMachine
{
    public Name? Name { get; set; }
    public Version? Version { get; set; }
    public Culture? Culture { get; set; }
    public Processor? Processor { get; set; }
    public Variables? Variables { get; set; }
    public List<Network>? Network { get; set; }
}