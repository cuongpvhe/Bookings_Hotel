using Microsoft.AspNetCore.Mvc;

namespace Bookings_Hotel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
