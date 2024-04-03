namespace Apito.Models.TheMachine;

using System.Net.NetworkInformation;
using System.Net.Sockets;

public class TheMachine
{
    public TheMachine()
    {
        Name = new Name();
        Version = new Version();
        Processor = new Processor();
        Variables = new Variables();
        Culture = new Culture();
        Network = [];
        try { AddNetworkMachines(); }
        catch (Exception) { Network.Add(new Network()); }
    }
    private void AddNetworkMachines()
    {
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up)
            {
                var networkMachine = new Network
                {
                    Id = nic.Id.Replace("{", "").Replace("}", ""),
                    Name = nic.Name,
                    Description = nic.Description,
                    Type = nic.NetworkInterfaceType.ToString()
                };

                var ip = nic.GetIPProperties().UnicastAddresses
                    .FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);
                if (ip != null)
                {
                    networkMachine.IpAddress = ip.Address.ToString();
                }

                var address = nic.GetPhysicalAddress();
                var mac = BitConverter.ToString(address.GetAddressBytes());
                networkMachine.Mac_Address = mac;
                networkMachine.MacAddress = address.ToString();

                Network.Add(networkMachine);
            }
        }
    }

    public Name Name { get; private set; }
    public Version Version { get; private set; }
    public Culture Culture { get; private set; }
    public Processor Processor { get; private set; }
    public Variables Variables { get; private set; }
    public List<Network> Network { get; private set; }
}