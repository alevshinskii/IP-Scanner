using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerLib
{
    public class Device
    {
        public string IPv4 { get; set; }

        public string IPv6 { get; set; }

        public string MacAddress { get; set; }

        public string Type { get; set; }

        public string Hostname { get; set; }

        public int Ping
        {
            get => Ping;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                Ping = value;
            }
        }
    }
}
