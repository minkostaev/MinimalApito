namespace Apito.Models.TheMachine;

public class Version
{
    public Version()
    {
        VersionString = Environment.OSVersion.VersionString;
        Platform = Environment.OSVersion.Platform.ToString();
        ServicePack = Environment.OSVersion.ServicePack;
        Build = Environment.OSVersion.Version.Build;
        Major = Environment.OSVersion.Version.Major;
        MajorRevision = Environment.OSVersion.Version.MajorRevision;
        Minor = Environment.OSVersion.Version.Minor;
        MinorRevision = Environment.OSVersion.Version.MinorRevision;
        Revision = Environment.OSVersion.Version.Revision;
    }

    public string VersionString { get; private set; }
    public string Platform { get; private set; }
    public string ServicePack { get; private set; }
    public int Build { get; private set; }
    public int Major { get; private set; }
    public int MajorRevision { get; private set; }
    public int Minor { get; private set; }
    internal int MinorRevision { get; private set; }
    public int Revision { get; private set; }
}