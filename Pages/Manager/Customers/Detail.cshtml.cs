using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    [Authorize(Policy = "StaffOnly")]
    public class DetailModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public DetailModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Account = await _context.Accounts.FindAsync(id);
            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
