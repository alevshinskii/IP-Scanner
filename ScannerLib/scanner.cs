using InterfaceLib;
using NetTools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using LoggerLib;

namespace ScannerLib
{

    public class NetScanner
    {
        private int oneTimePingLimit = 10;
        public ScannerSettings Settings { get; set; }
        private PingUtil pingUtil;
        private List<ILogger> loggers;

        public NetScanner(List<ILogger> loggers)
        {
            pingUtil = new PingUtil();
            this.loggers = loggers;
        }

        private void sendLogs(Log log)
        {
            foreach (var logger in loggers)
            {
                logger.SendLog(log);
            }
        }

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
                        IPv4 = reply.Address.ToString(),
                        Ping = reply.RoundtripTime
                    });
                }
            }
            return result;
        }

        public bool StartScan()
        {
            NetInterface netInterface = Settings.Interface;
            IPAddressRange range = new IPAddressRange(netInterface.Ip);
            if (netInterface.IpVersion == 4)
            {
                if (Settings.ScanType is ScanTypes.Subnet)
                {
                    if (IPAddress.TryParse(netInterface.Ip, out var ip)
                        && IPAddress.TryParse(netInterface.SubnetMask, out var subnet))
                    {
                        var maskLength = IPAddressRange.SubnetMaskLength(subnet);
                        range = new IPAddressRange(ip, maskLength);
                    }
                }
                else if (Settings.ScanType == ScanTypes.Range)
                {
                    if (IPAddress.TryParse(Settings.IPRangeBottom, out var bottom)
                        && IPAddress.TryParse(Settings.IPRangeTop, out var top))
                    {
                        range = new IPAddressRange(bottom, top);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                netInterface.Devices.AddRange(PingAsync(range).Result);
                foreach (var device in netInterface.Devices)
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
                                device.IPv6 = address.ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }

            sendLogs(new Log(netInterface));
            return true;
        }
    }

    public enum ScanTypes:int
    {
        Subnet=1,
        Range=2
    }
    public class ScannerSettings
    {
        public NetInterface Interface { get; set; }
        public ScanTypes ScanType { get; set; }
        public string IPRangeBottom { get; set; }
        public string IPRangeTop { get; set; }
        public int Interval { get; set; }

    }

}
