using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Threading.Tasks;
using System;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    public class EditModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public EditModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account Account { get; set; } = new Account();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Account = await _context.Accounts.FindAsync(id);

            if (Account == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                if (!_context.Accounts.Any(a => a.AccountId == Account.AccountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("List");
        }
    }
}
