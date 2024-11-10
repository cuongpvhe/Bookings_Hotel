using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages
{
    public class ServicesModel : PageModel
    {

        private readonly HotelBookingSystemContext _context;

        public ServicesModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<Bookings_Hotel.Models.Service> Services { get; set; }
        public async Task OnGetAsync()
        {
            Services = await _context.Services.Include(x => x.ServiceImages).Where(x => x.Status == ServiceStatus.ACTIVE).ToListAsync();
        }
    }
}
