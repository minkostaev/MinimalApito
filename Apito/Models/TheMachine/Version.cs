namespace Apito.Models.TheMachine;

public class Version
{
    public string? VersionString { get; set; }
    public string? Platform { get; set; }
    public string? ServicePack { get; set; }
    public int? Build { get; set; }
    public int? Major { get; set; }
    public int? MajorRevision { get; set; }
    public int? Minor { get; set; }
    internal int? MinorRevision { get; set; }
    public int? Revision { get; set; }
}