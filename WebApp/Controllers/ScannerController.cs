using System.Text.Json;
using InterfaceLib;
using LoggerLib;
using Microsoft.AspNetCore.Mvc;
using ScannerLib;
using ILogger = LoggerLib.ILogger;

namespace WebApp.Controllers
{
    public class ScannerController : Controller
    {
        private NetScanner? scanner;
        private InterfaceUtility? interfaceUtility;


        public IActionResult Index()
        {
            scanner = getScanner();

            return View(scanner);


        }
        private NetScanner getScanner()
        {
            var scanner = new NetScanner(new List<ILogger>(){new LocalLogger(new KuznechikCypher())});
            try
            {
                using (FileStream fs = new FileStream("scansettings.json", FileMode.OpenOrCreate))
                {
                    scanner.Settings = JsonSerializer.Deserialize<ScannerSettings>(fs);
                }
            }
            catch
            {
                //settings not found
            }
            return scanner;
        }

        [HttpGet]
        public IActionResult Settings()
        {
            interfaceUtility= new InterfaceUtility();
            ViewBag.Interfaces=interfaceUtility.Interfaces;
            ViewBag.ScanTypes=Enum.GetNames(typeof(ScanTypes)).ToList() ;
            return View();
        }

        [HttpPost]
        public IActionResult Settings(string netInterface, string scanType, string iprangebottom, string iprangetop,string interval)
        {
            interfaceUtility= new InterfaceUtility();
            ScannerSettings settings;
            try
            {
                settings = new ScannerSettings()
                {
                    Interface = interfaceUtility.Interfaces.First(i => i.Ip==netInterface),
                    ScanType = Enum.Parse<ScanTypes>(scanType),
                    IPRangeBottom = iprangebottom,
                    IPRangeTop = iprangetop,
                    Interval = int.Parse(interval)
                };
                using (FileStream fs = new FileStream("scansettings.json", FileMode.OpenOrCreate))
                {
                    JsonSerializer.Serialize<ScannerSettings>(fs, settings);
                }
            }
            catch
            {
                //cant create settings file
            }

            return RedirectToAction("Index");
        }

        public IActionResult Result()
        {
            scanner = getScanner();
            Task.Run(()=>
            {
                scanner.StartScan();
            }).Wait();
            var netInterface = scanner.Settings.Interface;
            ViewBag.Interface = netInterface.ToString();
            return View(netInterface.Devices);
        }
    }
}
