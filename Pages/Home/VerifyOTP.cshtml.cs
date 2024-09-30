using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Bookings_Hotel.Pages.Home
{
    public class VerifyOTPModel : PageModel
    {
        private readonly Booking_hotelContext _context;

        public VerifyOTPModel(Booking_hotelContext context)
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
                    var userJson = TempData.Peek("User") as string; // Lấy giá trị JSON của đối tượng User từ TempData
                    if (userJson != null)
                    {
                        // Deserialize đối tượng User từ JSON
                        User user = JsonSerializer.Deserialize<User>(userJson);

                        if (user != null)
                        {
                            user.Phonenumber = user.Phonenumber.Trim();
                            user.Username = user.Username.Trim();

                            _context.Users.Add(user);
                            _context.SaveChanges();
                            return RedirectToPage("/Home/Login");
                        }
                    }

                    ModelState.AddModelError("OTP", "Thông tin người dùng không hợp lệ.");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError("OTP", "Mã OTP không đúng vui lòng thử lại");
                    return Page();
                }
            }
            catch
            {
                ModelState.AddModelError("OTP", "Mã OTP Không hợp lệ !");
                return Page();
            }
        }

    }
}
