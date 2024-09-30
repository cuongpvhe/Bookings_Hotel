using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net;
using System.Net.Mail;

namespace Bookings_Hotel.Pages.Home
{
    public class VerifyOTPForgotPasswordModel : PageModel
    {
        [BindProperty]
        public int Otp { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                int storedOTP = Convert.ToInt32(TempData["OTP"]);

                if (Otp == storedOTP)
                {
                    Message = "OTP xác thực thành công";
                    TempData["UserID"] = TempData["UserID"];
                    return RedirectToPage("Datlaimatkhau"); // Chuyển đến trang đặt lại mật khẩu
                }
                else
                {
                    ModelState.AddModelError("Otp", "Mã OTP không đúng, vui lòng thử lại");
                    return Page();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Otp", "Mã OTP không hợp lệ !");
                return Page();
            }
        }
    }
}
