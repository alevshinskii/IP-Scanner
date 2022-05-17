using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace lib
{

    public class IpInterface
    {
        public string Ip { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Ip + " (" + Name + " - " + Description + ")";
        }
    }

    public class InterfaceUtility
    {
        public List<IpInterface> GetInterfaces()
        {
            var list = new List<IpInterface>();

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
                    foreach (IPAddressInformation addressInformation in properties.UnicastAddresses)
                    {
                        // We're only interested in IPv4 addresses for now
                        if (addressInformation.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        // Ignore loopback addresses (e.g., 127.0.0.1)
                        if (IPAddress.IsLoopback(addressInformation.Address))
                            continue;

                        list.Add(new IpInterface()
                        {
                            Ip=addressInformation.Address.ToString(),
                            Name = network.Name,
                            Description = network.Description
                        });
                    }
                }
            }

            return list;
        }
    }
}
