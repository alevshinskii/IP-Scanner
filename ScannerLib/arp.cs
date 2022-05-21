using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace ScannerLib
{
    public class ArpItem
    {
        public IPAddress Ip { get; set; }

        public string MacAddress { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return Ip + " - " + MacAddress + " - " + Type;
        }
    }
    public class ArpUtil
    {
        public void GetArpResult(INetInterface netInterface)
        {
            var devices = netInterface.Devices;
            using (Process process = Process.Start(new ProcessStartInfo("arp", "-a -N " + netInterface.Ip)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            }))
            {
                var output = process.StandardOutput.ReadToEnd();
                var arpItems = ParseArpResult(output);
                foreach (var arpItem in arpItems)
                {
                    if (devices.Any(d => d.MacAddress == arpItem.MacAddress))
                    {
                        foreach (var device in devices.Where(d => d.MacAddress == arpItem.MacAddress))
                        {
                            if (device.IPv4 is null)
                            {
                                device.IPv4 = arpItem.Ip;
                            }
                        }

                    }
                    else
                    {
                        devices.Add(new Device() { IPv4 = arpItem.Ip, MacAddress = arpItem.MacAddress });
                    }
                }
            }
        }

        private List<ArpItem> ParseArpResult(string output)
        {
            var lines = output.Split('\n');

            var result = from line in lines
                         let item = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         where item.Count() == 4
                         select new ArpItem()
                         {
                             Ip = IPAddress.Parse(item[0]),
                             MacAddress = item[1],
                             Type = item[2]
                         };

            return result.ToList();
        }
    }
}
