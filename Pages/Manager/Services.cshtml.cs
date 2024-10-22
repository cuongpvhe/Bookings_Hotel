using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager
{
    public class ServicesModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public ServicesModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        //List
        public List<string>? TableHeaders { get; set; } = new List<string> { ".No", "Service Id", "Service Name", "Created Date", "Update Date", "Price", "Actions" };
        public List<Service> Services { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Services = await _context.Services.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Services = await _context.Services.ToListAsync();
            }
            else
            {
                Services = await _context.Services.Where(x => x.ServiceName.Contains(searchTerm)).ToListAsync();

            }
            return Partial("PartialViews/Manager/_ServicesPartialView", this);
        }
    }
}
