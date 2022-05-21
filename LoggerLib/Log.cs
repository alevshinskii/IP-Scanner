using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InterfaceLib;

namespace LoggerLib
{
    public class Log
    {
        public Log(INetInterface netInterface)
        {
            NetInterface=netInterface;
            TimeStamp=DateTime.Now;
            ChangesMessage = "No changes";
        }
        public INetInterface NetInterface { get; }
        public DateTime TimeStamp { get; }
        public string ChangesMessage { get; set; }

    }
}
