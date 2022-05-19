using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ScannerLib
{
    public interface INetInterface
    {
        public string Ip { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Device> Devices { get; set; }
    }

    public class Ipv4Interface : INetInterface
    {
        public string Ip { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Device> Devices { get; set; }
        public string SubnetMask { get; set; }

        public override string ToString()
        {
            return "IP: " + Ip + " (" + Name + " - " + Description + ")";
        }
    }
    public class Ipv6Interface : INetInterface
    {
        public string Ip { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Device> Devices { get; set; }

        public override string ToString()
        {
            return "IP: " + Ip + " (" + Name + " - " + Description + ")";
        }
    }

    public class InterfaceUtility
    {
        public List<INetInterface> GetInterfaces()
        {
            var list = new List<INetInterface>();

            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection)
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network
                IPInterfaceProperties properties = network.GetIPProperties();

                if (network.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    network.OperationalStatus == OperationalStatus.Up &&
                    !network.Description.ToLower().Contains("virtual") &&
                    !network.Description.ToLower().Contains("pseudo"))
                {
                    // Each network interface may have multiple IP addresses
                    foreach (UnicastIPAddressInformation addressInformation in properties.UnicastAddresses)
                    {
                        string ip = addressInformation.Address.ToString();
                        // Ignore loopback addresses
                        if (IPAddress.IsLoopback(addressInformation.Address))
                            continue;
                        // Ignore link-local
                        if (addressInformation.Address.IsIPv6LinkLocal)
                            continue;
                        if (addressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            list.Add(new Ipv4Interface()
                            {
                                Ip = ip,
                                Name = network.Name,
                                Description = network.Description,
                                SubnetMask = addressInformation.IPv4Mask.ToString()
                            });
                        }
                        else if (addressInformation.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            list.Add(new Ipv6Interface()
                            {
                                Ip = ip,
                                Name = network.Name,
                                Description = network.Description
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
