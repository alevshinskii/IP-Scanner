using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class LogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
