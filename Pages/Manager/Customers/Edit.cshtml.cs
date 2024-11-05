using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    [Authorize(Policy = "StaffOnly")]
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
            var accountToUpdate = await _context.Accounts.FindAsync(Account.AccountId);

            if (accountToUpdate == null)
            {
                return NotFound();
            }
            if (await _context.Accounts.AnyAsync(a => a.Email == Account.Email && a.AccountId != accountToUpdate.AccountId))
            {
                ModelState.AddModelError("Account.Email", "Email đã tồn tại.");
                return Page();
            }

            // Kiểm tra tính duy nhất của UseName
            if (await _context.Accounts.AnyAsync(a => a.UseName == Account.UseName && a.AccountId != accountToUpdate.AccountId))
            {
                ModelState.AddModelError("Account.UseName", "Tài khoản đã tồn tại.");
                return Page();
            }
            if (await _context.Accounts.AnyAsync(a => a.FullName == Account.FullName && a.AccountId != accountToUpdate.AccountId))
            {
                ModelState.AddModelError("Account.FullName", "Họ và tên đã tồn tại.");
                return Page();
            }
            if (await _context.Accounts.AnyAsync(a => a.Phonenumber == Account.Phonenumber && a.AccountId != accountToUpdate.AccountId))
            {
                ModelState.AddModelError("Account.Phonenumber", "Số điện thoại đã tồn tại.");
                return Page();
            }
           

        
            accountToUpdate.FullName = Account.FullName;
            accountToUpdate.Email = Account.Email;
            accountToUpdate.Phonenumber = Account.Phonenumber;
            accountToUpdate.Gender = Account.Gender;
            accountToUpdate.Address = Account.Address;

            if (string.IsNullOrEmpty(Account.Status))
            {
                Account.Status = accountToUpdate.Status; 
            }

            
            accountToUpdate.UpdateDate = DateTime.Now;

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
