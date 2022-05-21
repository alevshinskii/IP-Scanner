using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ScannerLib
{

    public class PingUtil
    {
        public PingReply GetPingResult(string ip)
        {
            Ping ping = new Ping();
            PingReply pingresult = ping.Send(ip);
            if (pingresult!=null && pingresult.Status == IPStatus.Success)
            {
                return pingresult;
            }
            return null;
        }

    }
}
