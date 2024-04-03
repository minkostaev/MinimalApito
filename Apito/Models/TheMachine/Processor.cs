namespace Apito.Models.TheMachine;

public class Processor
{
    public Processor()
    {
        Is64BitOS = Environment.Is64BitOperatingSystem;
        Is64BitProcess = Environment.Is64BitProcess;
        ProcessorCount = Environment.ProcessorCount;
    }

    public bool Is64BitOS { get; private set; }
    public bool Is64BitProcess { get; private set; }
    public int ProcessorCount { get; private set; }
}