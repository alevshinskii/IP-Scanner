using ScannerLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (var ipInterface in new InterfaceUtility().GetInterfaces())
            {
                if (ipInterface is Ipv4Interface ipv4Interface && ipv4Interface.Name == "Ethernet")
                {
                    IPv4Scanner scanner= new IPv4Scanner();
                    scanner.Settings = new ScannerSettings()
                        { Interface = ipv4Interface, ScanType = "Subnet"};
                    scanner.StartScan();
                    foreach (var device in ipv4Interface.Devices)
                    {
                        Console.WriteLine(device.ToString());
                    }
                }


            }



            Console.ReadKey();




        }
    }
}