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
        public int OTP { get; set; }

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

                if (OTP == storedOTP)
                {
                    Message = "OTP xác thực thành công";

                    // Lưu lại email của người dùng để chuyển qua ResetPassword
                    TempData["Email"] = TempData["Email"];
                    TempData.Keep("Email"); // Đảm bảo giữ lại email sau redirect

                    return RedirectToPage("/Home/ResetPassword"); // Chuyển đến trang đặt lại mật khẩu
                }
                else
                {
                    ModelState.AddModelError("OTP", "Mã OTP không đúng, vui lòng thử lại");
                    return Page();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("OTP", "Mã OTP không hợp lệ !");
                return Page();
            }
        }

    }
}
