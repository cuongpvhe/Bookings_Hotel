using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return Page();
            }

            // Cập nhật mật khẩu cho người dùng
            // ...

            return RedirectToPage("Login");
        }
    }
}
