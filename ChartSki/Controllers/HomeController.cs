using Microsoft.AspNetCore.Mvc;

namespace ChartSki.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
