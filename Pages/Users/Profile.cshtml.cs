using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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
        public Bookings_Hotel.Models.Account Account { get; set; }

        [BindProperty]
        public IFormFile AvatarUpload { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy AccountId từ session
            int? accountId = int.TryParse(HttpContext.Session.GetString("AccountId"), out var id) ? id : (int?)null;


            // Kiểm tra xem accountId có null không
            if (accountId == null)
            {
                // Trả về NotFound nếu accountId không hợp lệ
                return NotFound();
            }

            // Tìm tài khoản trong cơ sở dữ liệu
            Account = await _context.Accounts.FindAsync(accountId);

            // Kiểm tra xem tài khoản có tồn tại không
            if (Account == null)
            {
                // Trả về NotFound nếu không tìm thấy tài khoản
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

            var accountInDb = await _context.Accounts.FindAsync(Account.AccountId);

            if (accountInDb == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin của người dùng
            accountInDb.FullName = Account.FullName;
            accountInDb.Dob = Account.Dob;
            accountInDb.Phonenumber = Account.Phonenumber;
            accountInDb.Gender = Account.Gender;
            accountInDb.Address = Account.Address;

            // Xử lý tải ảnh Avatar
            if (AvatarUpload != null)
            {
                var fileName = $"{accountInDb.AccountId}_{Path.GetFileName(AvatarUpload.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarUpload.CopyToAsync(fileStream);
                }

                accountInDb.Avatar = $"/uploads/{fileName}";
            }

            accountInDb.UpdateDate = DateTime.Now;

            _context.Attach(accountInDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(Account.AccountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Users/Profile");
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
