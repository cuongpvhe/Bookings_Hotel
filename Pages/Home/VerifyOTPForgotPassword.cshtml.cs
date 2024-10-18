using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;

namespace Bookings_Hotel.Pages.Home
{
    public class VerifyOTPForgotPasswordModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        [BindProperty]
        public int OTP { get; set; }

        public string Message { get; set; }

        public VerifyOTPForgotPasswordModel(HotelBookingSystemContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost(int otp)
        {
            try
            {
                int storedOTP = Convert.ToInt32(TempData.Peek("OTP")); // Sử dụng Peek() để giữ lại giá trị OTP
                if (otp == storedOTP)
                {
                    // Lấy Account_ID từ TempData
                    int? accountId = TempData.Peek("Account_ID") as int?;

                    if (accountId != null)
                    {
                        // Truy vấn để lấy tài khoản tương ứng từ cơ sở dữ liệu
                        var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);

                        if (account != null)
                        {
                            // Sau khi xác thực thành công, lưu email người dùng vào TempData để chuyển hướng đến trang ResetPassword
                            TempData["Email"] = account.Email;

                            // Chuyển hướng đến trang ResetPassword để nhập mật khẩu mới
                            return RedirectToPage("/Home/ResetPassword");
                        }
                    }

                    ModelState.AddModelError("OTP", "Thông tin tài khoản không hợp lệ.");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("OTP", "Mã OTP không đúng, vui lòng thử lại.");
                    return Page();
                }
            }
            catch
            {
                ModelState.AddModelError("OTP", "Mã OTP không hợp lệ!");
                return Page();
            }
        }

    }
}
