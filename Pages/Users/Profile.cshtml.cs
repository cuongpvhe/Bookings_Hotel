using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
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
            // Lấy AccountId từ Claims
            int? accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value != null
                ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value)
                : (int?)null;

            if (accountId == null)
            {
                return NotFound();
            }

            // Tìm tài khoản trong CSDL
            account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            return Page();
        }

        // Phương thức để lưu thay đổi khi người dùng nhấn nút "Save Changes"
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lấy AccountId từ Claims
            int? accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value != null
                ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value)
                : (int?)null;

            if (accountId == null)
            {
                return NotFound();
            }

            // Tìm tài khoản trong CSDL
            var accountToUpdate = await _context.Accounts.FindAsync(accountId);

            if (accountToUpdate == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin tài khoản
            accountToUpdate.FullName = account.FullName;
            accountToUpdate.Dob = account.Dob;
            accountToUpdate.Phonenumber = account.Phonenumber;
            accountToUpdate.Gender = account.Gender;
            accountToUpdate.Address = account.Address;

            // Lưu thay đổi vào CSDL
            await _context.SaveChangesAsync();

            return RedirectToPage("/Users/Profile");
        }
    }
}
