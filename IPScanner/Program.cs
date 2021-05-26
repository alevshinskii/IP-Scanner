using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using IPv4_Scanner;
using IPv6_Scanner;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var ipv4scan = new IPv4Scanner();
            //ipv4scan.Start("192.168.1.1", true);
            //var ipv6scan=new IPv6Scanner();
            //ipv6scan.Start("fd01::1");
        }
    }
}
