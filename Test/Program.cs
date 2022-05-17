﻿using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using lib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //ArpUtil arpHelper = new ArpUtil();
            //PingUtil pingUtil = new PingUtil();
            //List<ArpItem> arpEntities = arpHelper.GetArpResult();
            //foreach (var item in arpEntities)
            //{
            //    Console.WriteLine(item.Ip + "\t" + item.MacAddress + "\t" + item.Type);

            //    Console.WriteLine(pingUtil.GetPingResult(item.Ip));
            //}
            //Console.ReadLine();


            //String strHostName = string.Empty;
            //// Getting Ip address of local machine...
            //// First get the host name of local machine.
            //strHostName = Dns.GetHostName();
            //Console.WriteLine("Local Machine's Host Name: " + strHostName);
            //// Then using host name, get the IP address list..
            //IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            //IPAddress[] addr = ipEntry.AddressList;

            //for (int i = 0; i < addr.Length; i++)
            //{
            //    Console.WriteLine("IP Address {0}: {1} ", i, addr[i]);
            //}
            //Console.ReadLine();

            foreach (var ipInterface in new InterfaceUtility().GetInterfaces())
            {
                Console.WriteLine(ipInterface.ToString());
            }

            foreach (var arpItem in new ArpUtil().GetArpResult())
            {
                Console.WriteLine(arpItem.ToString());
            }

        }
    }
}