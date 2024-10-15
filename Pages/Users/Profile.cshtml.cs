using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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
        public Bookings_Hotel.Models.Account account { get; set; }

        [BindProperty]
        public IFormFile AvatarUpload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();  // Nếu dữ liệu không hợp lệ, trả về trang hiện tại
            }

            // Lấy AccountId từ Claims
            int? accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value != null
                ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value)
                : (int?)null;

            if (accountId == null)
            {
                return NotFound();  // Nếu không có AccountId, trả về NotFound
            }

            var accountInDb = await _context.Accounts.FindAsync(accountId);

            if (accountInDb == null)
            {
                return NotFound();  // Nếu không tìm thấy tài khoản trong CSDL, trả về NotFound
            }

            // Cập nhật thông tin của người dùng
            accountInDb.FullName = account.FullName;
            accountInDb.Dob = account.Dob;
            accountInDb.Phonenumber = account.Phonenumber;
            accountInDb.Gender = account.Gender;
            accountInDb.Address = account.Address;

            // Xử lý tải ảnh Avatar
            if (AvatarUpload != null)
            {
                var fileName = $"{accountInDb.AccountId}_{Path.GetFileName(AvatarUpload.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarUpload.CopyToAsync(fileStream);  // Lưu ảnh lên server
                }

                accountInDb.Avatar = $"/uploads/{fileName}";  // Cập nhật đường dẫn Avatar
            }

            accountInDb.UpdateDate = DateTime.Now;

            // Không cần gán lại State, vì _context đã theo dõi accountInDb
            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(accountId.Value))
                {
                    return NotFound();  // Nếu xảy ra lỗi trong quá trình cập nhật, kiểm tra lại tồn tại của Account
                }
                else
                {
                    throw;  // Ném lỗi nếu xảy ra vấn đề khác
                }
            }

            return RedirectToPage("/Users/Profile");  // Chuyển hướng về trang Profile sau khi cập nhật thành công
        }


        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
