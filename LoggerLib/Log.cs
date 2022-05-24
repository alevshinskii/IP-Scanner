using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InterfaceLib;

namespace LoggerLib
{
    public class Log
    {
        public Log(NetInterface netInterface)
        {
            NetInterface=netInterface;
            TimeStamp=DateTime.Now;
            ChangesMessage = "No changes";
        }
        public NetInterface NetInterface { get; }
        public DateTime TimeStamp { get; }
        public string ChangesMessage { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine();
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("Time: "+ TimeStamp);
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("Changes: " + ChangesMessage);
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("Interface: ");
            sb.AppendLine(NetInterface.ToString());
            sb.AppendLine("Devices: ");
            if (NetInterface.Devices.Count > 0)
            {
                foreach (var device in NetInterface.Devices)
                {
                    sb.AppendLine(device.ToString());
                }
            }
            else
            {
                sb.AppendLine("No devices");
            }
            sb.AppendLine("---------------------------------------------------------");
            return sb.ToString();
        }
    }
}
