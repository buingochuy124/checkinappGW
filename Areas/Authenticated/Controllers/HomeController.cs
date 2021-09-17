using Microsoft.AspNetCore.Mvc;

namespace CheckInGWDN.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Tutorial()
        {
            return View();
        }

        public IActionResult AddStudent()
        {
            return View();
        }

        public IActionResult QrCode()
        {
            return View();
        }

        public IActionResult ErrorPage()
        {
            return View();
        }
    }
}