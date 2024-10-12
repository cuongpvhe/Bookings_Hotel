using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Bookings_Hotel.Pages.Home
{
    public class LoginModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public LoginModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            // Tìm kiếm người dùng theo Username và Password (có thể thêm logic mã hóa mật khẩu)
            var account = _context.Accounts.FirstOrDefault(a => a.UseName == Username.Trim() && a.Password == Password);

            if (account != null)
            {
                // Lưu thông tin tối thiểu của tài khoản vào session
                HttpContext.Session.SetString("AccountId", account.AccountId.ToString());
                HttpContext.Session.SetString("UseName", account.UseName);
                HttpContext.Session.SetInt32("RoleId", (int)account.RoleId);

                // Chuyển hướng theo vai trò của người dùng
                if (account.RoleId == 2) // Người dùng thông thường
                {
                    return RedirectToPage("/Index");
                }
                else if (account.RoleId == 1) // Admin
                {
                    return RedirectToPage("Admin/Managers");
                }
                else if (account.RoleId == 3) // Nhân viên
                {
                    return RedirectToPage("Staff/Managers");
                }
            }

            // Thông báo lỗi khi thông tin đăng nhập không hợp lệ
            ModelState.AddModelError("Error_Login", "Invalid username or password.");
            return Page();
        }
    }
}
