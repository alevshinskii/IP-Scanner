using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ScannerLib
{
    public class PingItem
    {
        public string Ip { get; set; }

        public string MacAddress { get; set; }

        public string Type { get; set; }
    }


    public class PingUtil
    {
        public async void GetPingResult(string ip)
        {
            Ping pingSender = new Ping ();
            PingOptions options = new PingOptions ();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;
            options.Ttl = 256;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 300;
            PingReply reply = await pingSender.SendPingAsync(ip, timeout, buffer, options);
            
        }

    }
}
