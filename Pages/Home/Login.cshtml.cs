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
                // Kiểm tra nếu là tài khoản người dùng thông thường
                if (account.RoleId == 1)
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.SetString("User", account.AccountId.ToString());
                    return RedirectToPage("/Index");
                }
                // Kiểm tra nếu là tài khoản admin
                else if (account.RoleId == 2)
                {
                    HttpContext.Session.SetString("admin", account.AccountId.ToString());
                    return RedirectToPage("Admin/Managers");
                }
                // Kiểm tra nếu là tài khoản nhân viên
                else if (account.RoleId == 3)
                {
                    HttpContext.Session.SetString("staff", account.AccountId.ToString());
                    return RedirectToPage("Staff/Managers");
                }
            }

            // Thông báo lỗi khi thông tin đăng nhập không hợp lệ
            ModelState.AddModelError("Error_Login", "Invalid username or password.");
            return Page();
        }
    }
}
