using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Threading.Tasks;
using System;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    public class CreateModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public CreateModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account Account { get; set; } = new Account();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Account.RoleId = 2;
            Account.CreatedDate = DateTime.Now;
            Account.Status = "Active";
            
            _context.Accounts.Add(Account);
            await _context.SaveChangesAsync();

            return RedirectToPage("List");
        }
    }
}
