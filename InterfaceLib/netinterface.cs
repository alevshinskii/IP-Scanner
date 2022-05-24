using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace InterfaceLib
{
    public class NetInterface
    {
        public string Ip { get; set; }
        public int IpVersion { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Device> Devices { get; set; }=new List<Device>();
        public string SubnetMask { get; set; }

        public override string ToString()
        {
            if(IpVersion==4)
                return "IP: " + Ip + " (" + Name + " - " + Description + ") Mask: " + SubnetMask;
            else
                return "IP: " + Ip + " (" + Name + " - " + Description + ")";
        }
    }


    public class InterfaceUtility
    {
        public List<NetInterface> Interfaces { get; }

        public InterfaceUtility()
        {
            Interfaces = GetInterfaces();
        }
        private List<NetInterface> GetInterfaces()
        {
            var list = new List<NetInterface>();

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
                            list.Add(new NetInterface()
                            {
                                Ip = ip,
                                IpVersion = 4,
                                Name = network.Name,
                                Description = network.Description,
                                SubnetMask = addressInformation.IPv4Mask.ToString()
                            });
                        }
                        else
                        {
                            list.Add(new NetInterface()
                            {
                                Ip = ip,
                                IpVersion = 6,
                                Name = network.Name,
                                Description = network.Description,
                                SubnetMask = addressInformation.IPv4Mask.ToString()
                            });
                        }

                    }
                }
            }

            return list;
        }
    }
}
