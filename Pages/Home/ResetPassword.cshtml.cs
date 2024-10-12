using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace Bookings_Hotel.Pages.Home
{
    public class ResetPasswordModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public ResetPasswordModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public IActionResult OnPost()
        {
            // Lấy email từ TempData
            string userEmail = TempData.Peek("Email") as string;  // Sử dụng Peek để giữ lại email

            if (string.IsNullOrEmpty(userEmail))
            {
                ModelState.AddModelError("", "Thông tin người dùng không hợp lệ.");
                return Page();
            }

            // Kiểm tra xem mật khẩu mới và mật khẩu xác nhận có khớp không
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return Page();
            }

            // Tìm người dùng trong cơ sở dữ liệu
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return Page();
            }

            // Lưu mật khẩu mới vào cơ sở dữ liệu (không hash mật khẩu)
            user.Password = NewPassword;

            try
            {
                // Cập nhật thông tin trong cơ sở dữ liệu
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi cập nhật mật khẩu: {ex.Message}");
                return Page();
            }

            // Sau khi mật khẩu đã được cập nhật thành công, chuyển hướng đến trang đăng nhập
            TempData["SuccessMessage"] = "Mật khẩu của bạn đã được cập nhật thành công.";
            return RedirectToPage("/Home/Login");
        }
    }
}
