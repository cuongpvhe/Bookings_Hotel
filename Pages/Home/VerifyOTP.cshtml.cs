using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Bookings_Hotel.Pages.Home
{
    public class VerifyOTPModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public VerifyOTPModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int OTP { get; set; }

        public IActionResult OnPost(int otp)
        {
            try
            {
                int storedOTP = Convert.ToInt32(TempData.Peek("OTP")); // Sử dụng Peek() để giữ lại giá trị OTP
                if (otp == storedOTP)
                {
                    var accountJson = TempData.Peek("Account") as string; // Lấy giá trị JSON của đối tượng Account từ TempData
                    if (accountJson != null)
                    {
                        // Deserialize đối tượng Account từ JSON
                        Account account = JsonSerializer.Deserialize<Account>(accountJson);

                        if (account != null)
                        {
                            account.Phonenumber = account.Phonenumber.Trim();
                            account.UseName = account.UseName.Trim();

                            _context.Accounts.Add(account); // Thêm tài khoản vào bảng Accounts
                            _context.SaveChanges();
                            return RedirectToPage("/Home/Login");
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
