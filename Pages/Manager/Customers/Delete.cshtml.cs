using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Threading.Tasks;
using System;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public DeleteModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Account = await _context.Accounts.FindAsync(id);

            if (Account != null)
            {
                _context.Accounts.Remove(Account);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("List");
        }
    }
}
