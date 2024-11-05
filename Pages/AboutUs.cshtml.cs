using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages
{
    public class AboutUsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public AboutUsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int NumberRoom { get; set; }

        [BindProperty]
        public int NumberService { get; set; }

        [BindProperty]
        public int NumberCustomer { get; set; }
        public void OnGet()
        {
            NumberRoom = _context.Rooms.ToList().Count;
            NumberService = _context.Services.ToList().Count;
            NumberCustomer = _context.Accounts.Where(a => a.Role.RoleName == RoleName.CUSTOMER).ToList().Count;
        }
    }
}
