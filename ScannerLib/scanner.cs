using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetTools;

namespace ScannerLib
{

    public interface IScanner
    {
        void StartScan();
    }
    public class IPv4Scanner:IScanner
    {
        private int oneTimePingLimit = 10;
        public ScannerSettings Settings { get; set; }
        private PingUtil pingUtil = new PingUtil();

        private async Task<List<Device>> PingAsync(IPAddressRange range)
        {
            var result = new List<Device>();
            var tasks = new List<Task>();
            foreach (IPAddress ip in range)
            {
                tasks.Add(new Ping().SendPingAsync(ip.ToString()));
            }
            await Task.WhenAll(tasks);

            List<PingReply> replies = new List<PingReply>();
            foreach (Task<PingReply> task in tasks)
            {
                replies.Add(task.Result);
            }


            foreach (var reply in replies)
            {
                if (reply.Status == IPStatus.Success)
                {
                    result.Add(new Device()
                    {
                        IPv4 = reply.Address,
                        Ping = reply.RoundtripTime
                    });
                }
            }
            return result;
        }

        public void StartScan()
        {
            if (Settings.Interface is Ipv4Interface ipv4Interface)
            {
                IPAddressRange range=new IPAddressRange();
                if (Settings.ScanType == "Subnet")
                {
                    if (IPAddress.TryParse(ipv4Interface.Ip, out var ip)
                        && IPAddress.TryParse(ipv4Interface.SubnetMask, out var subnet))
                    {
                        var maskLength = IPAddressRange.SubnetMaskLength(subnet);
                        range = new IPAddressRange(ip,maskLength);
                    }
                }
                else if (Settings.ScanType == "Range")
                {
                    if (IPAddress.TryParse(Settings.IPRangeBottom,out var bottom) 
                        && IPAddress.TryParse(Settings.IPRangeTop, out var top))
                    {
                        range = new IPAddressRange(bottom,top);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                ipv4Interface.Devices.AddRange(PingAsync(range).Result);
                foreach (var device in ipv4Interface.Devices)
                {
                    IPHostEntry entry = null;
                    try
                    {
                        entry = Dns.GetHostEntry(device.IPv4);
                    }
                    catch
                    {
                        //cant resolve information about host
                    }

                    if (entry != null)
                    {
                        device.Hostname = entry.HostName;
                        foreach (var address in entry.AddressList)
                        {
                            if (address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                device.IPv6 = address;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
    public class IPv6Scanner:IScanner
    {
        public ScannerSettings Settings { get; set; }

        public void StartScan()
        {
            throw new NotImplementedException();
        }
    }

    public class ScannerSettings
    {
        public INetInterface Interface { get; set; }
        public string ScanType { get; set; }
        public string IPRangeBottom { get; set; }
        public string IPRangeTop { get; set; }
        public int Interval { get; set; }

    }

}
