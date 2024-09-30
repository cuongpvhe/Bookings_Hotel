using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace Bookings_Hotel.Controllers
{
    public class HomeController : Controller
    {
        private Booking_hotelContext _booking_hotelContext;
        public IActionResult Index()
        {
            return View();
        }


        


    }
}
