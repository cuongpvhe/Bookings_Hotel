using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings_Hotel.Pages.Users
{
    public class ProfileModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public ProfileModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account account { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value != null
                ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value)
                : (int?)null;

            if (accountId == null)
            {
                return NotFound();
            }

            account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return Page();
            }

            int? accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value != null
                ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value)
                : (int?)null;

            if (accountId == null)
            {
                return NotFound();
            }

            var accountToUpdate = await _context.Accounts.FindAsync(accountId);

            if (accountToUpdate == null)
            {
                return NotFound();
            }

            // Log account details before updating
            Console.WriteLine($"Account Info - FullName: {account.FullName}, Dob: {account.Dob}, Email: {account.Email}, Phonenumber: {account.Phonenumber}, Gender: {account.Gender}, Address: {account.Address}");


            var newAccount = new Account
            {
                FullName = account.FullName,
                Dob = account.Dob,
                Email = account.Email,
                Phonenumber = account.Phonenumber,
                Gender = account.Gender,
                Address = account.Address
            };

            _context.Accounts.Update(newAccount);
            await _context.SaveChangesAsync();


            return RedirectToPage("/Users/Profile");
        }
    }
}
