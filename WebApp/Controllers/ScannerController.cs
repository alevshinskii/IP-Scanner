using Microsoft.AspNetCore.Mvc;
using ScannerLib;

namespace WebApp.Controllers
{
    public class ScannerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Settings()
        {
            InterfaceUtility interfaceUtility=new InterfaceUtility();
            ViewBag.Interfaces=interfaceUtility.GetInterfaces();
            return View();
        }
    }
}
