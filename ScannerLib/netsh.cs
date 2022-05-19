using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ScannerLib
{
    public class IPv6Item
    {
        public string Ip { get; set; }

        public string MacAddress { get; set; }

        public string Type { get; set; }
    }

    public class IPv6Utility
    {
        public List<IPv6Item> GetIPv6Result(Ipv6Interface netInterface)
        {
            using (Process process = Process.Start(new ProcessStartInfo("netsh", "interface ipv6 show neighbors interface='"+netInterface.Name+"'")
                   {
                       CreateNoWindow = true,
                       UseShellExecute = false,
                       RedirectStandardOutput = true
                   }))
            {
                var output = process.StandardOutput.ReadToEnd();
                return ParseIPv6Output(output);
            }
        }

        private List<IPv6Item> ParseIPv6Output(string output)
        {
            var result=new List<IPv6Item>();
            
            var lines = output.Split('\n');

            foreach (var line in lines)
            {
                
            }


            return result;
        }

    }
}
