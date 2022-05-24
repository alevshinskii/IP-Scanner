using System.Net;
using System.Text;

namespace InterfaceLib
{
    public class Device
    {
        public Device()
        {
            Hostname = "???";
            Ping = -1;
        }
        public string IPv4 { get; set; }

        public string IPv6 { get; set; }

        public string MacAddress { get; set; }

        public string Type { get; set; }

        public string Hostname { get; set; }

        public long Ping { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IPv4 != null)
            {
                sb.Append(IPv4+" ");
            }
            if (IPv6 != null)
            {
                sb.Append("(" + IPv6 + ") ");
            }
            if (Hostname != null)
            {
                sb.Append("Hostname: " + Hostname + " ");
            }
            if (MacAddress != null)
            {
                sb.Append("MacAddress: " + MacAddress + " ");
            }
            if (Type != null)
            {
                sb.Append("Type: " + Type + " ");
            }
            if (Ping >= 0)
            {
                sb.Append("Ping: " + Ping + "ms");
            }
            else
            {
                sb.Append("Ping: can't ping");
            }


            return sb.ToString();
        }
    }
}
