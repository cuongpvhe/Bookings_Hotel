using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Bookings_Hotel.Pages.Home
{
    public class RegisterModel : PageModel
    {
        private readonly Booking_hotelContext _context;

        [BindProperty]
        public User User { get; set; }

        public void OnGet()
        {
            if (TempData.ContainsKey("User"))
            {
                // Deserialize đối tượng User từ JSON
                User = JsonSerializer.Deserialize<User>(TempData["User"].ToString());
            }
        }

        public RegisterModel(Booking_hotelContext context)
        {
            _context = context;
        }
        public string ConfirmPassword { get; set; }


        public IActionResult OnPost(string ConfirmPassword)
        {
            // Kiểm tra sự phù hợp của mật khẩu
            if (User.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password is incorrect");
                return Page();
            }

            // Kiểm tra email
            if (!IsValidEmail(User.Email))
            {
                ModelState.AddModelError("Email", "Invalid email address.");
                return Page();
            }

            // Kiểm tra số điện thoại
            if (!IsValidPhoneNumber(User.Phonenumber))
            {
                ModelState.AddModelError("Phonenumber", "Invalid phone number.");
                return Page();
            }

            // Kiểm tra sự tồn tại của tên đăng nhập
            if (_context.Users.Any(u => u.Username == User.Username.ToLower().Trim()))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
               
                return Page();
            }

            // Tạo OTP và gửi qua email
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            if (SendOTP(User.Email, otp))
            {
                // Lưu thông tin người dùng vào TempData
                TempData["OTP"] = otp;
                TempData["User"] = JsonSerializer.Serialize(User);

                // Chuyển hướng đến trang xác thực OTP
                return RedirectToPage("/Home/VerifyOTP");
            }

            ModelState.AddModelError("", "Không thể gửi OTP. Vui lòng thử lại."); // Thông báo lỗi gửi OTP
            return Page();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{10,11}$");
        }

        private bool SendOTP(string email, int otp)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("phamduycuong2k1@gmail.com", "krowpvhmjfsndfum");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("phamduycuong2k1@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Your OTP for Registration";
                mailMessage.Body = "Your OTP is: " + otp.ToString();

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
