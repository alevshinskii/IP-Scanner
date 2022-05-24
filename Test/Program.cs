using System.Text;
using System.Text.Json;
using InterfaceLib;
using LoggerLib;
using ScannerLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            KuznechikCypher cypher = new KuznechikCypher();
            NetScanner scanner;
            LocalLogger logger = new LocalLogger(cypher);
            string serialized = "";
            foreach (var netInterface in new InterfaceUtility().Interfaces)
            {
                if (netInterface.Name == "Ethernet" && netInterface.IpVersion == 4)
                {
                    scanner = new NetScanner(new List<ILogger>() { logger });
                    scanner.Settings = new ScannerSettings()
                    { Interface = netInterface, ScanType = ScanTypes.Subnet };
                    scanner.StartScan();
                    foreach (var device in netInterface.Devices)
                    {
                        Console.WriteLine(device.ToString());
                    }


                }
            }


            foreach (var log in logger.RecieveLogs())
            {
                Console.WriteLine(log.ToString());
            }


            Console.ReadKey();
        }
    }
}