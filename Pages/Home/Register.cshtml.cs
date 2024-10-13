using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private readonly HotelBookingSystemContext _context;
        private readonly IConfiguration _configuration; // Để lấy thông tin cấu hình SMTP

        [BindProperty]
        public Account Account { get; set; }  // Thay User thành Account

        public string ConfirmPassword { get; set; }

        public RegisterModel(HotelBookingSystemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void OnGet()
        {
            if (TempData.ContainsKey("Account"))
            {
                // Deserialize đối tượng Account từ JSON
                Account = JsonSerializer.Deserialize<Account>(TempData["Account"].ToString());
            }
        }

        public IActionResult OnPost(string ConfirmPassword)
        {
            // Kiểm tra sự phù hợp của mật khẩu
            if (Account.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password confirmation does not match.");
                return Page();
            }

            // Kiểm tra email hợp lệ
            if (!IsValidEmail(Account.Email))
            {
                ModelState.AddModelError("Email", "Invalid email address.");
                return Page();
            }

            // Kiểm tra số điện thoại hợp lệ
            if (!IsValidPhoneNumber(Account.Phonenumber))
            {
                ModelState.AddModelError("Phonenumber", "Invalid phone number.");
                return Page();
            }

            // Kiểm tra sự tồn tại của tên đăng nhập hoặc email
            if (_context.Accounts.Any(a => a.UseName == Account.UseName.ToLower().Trim()))
            {
                ModelState.AddModelError("UseName", "Username already exists.");
                return Page();
            }

            if (_context.Accounts.Any(a => a.Email == Account.Email.ToLower().Trim()))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return Page();
            }

            // Tạo OTP và gửi qua email
            Random random = new Random();
            int otp = random.Next(100000, 999999);

            if (SendOTP(Account.Email, otp))
            {
                // Lưu thông tin tài khoản và OTP vào TempData
                TempData["OTP"] = otp;
                TempData["Account"] = JsonSerializer.Serialize(Account);

                // Chuyển hướng đến trang xác thực OTP
                return RedirectToPage("/Home/VerifyOTP");
            }

            ModelState.AddModelError("", "Failed to send OTP. Please try again."); // Thông báo lỗi gửi OTP
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
            // Kiểm tra số điện thoại hợp lệ (10 hoặc 11 chữ số)
            return Regex.IsMatch(phoneNumber, @"^\d{10,11}$");
        }

        private bool SendOTP(string email, int otp)
        {
            try
            {
                // Lấy thông tin cấu hình SMTP từ appsettings.json
                var smtpServer = _configuration["Smtp:Server"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"]);
                var smtpUser = _configuration["Smtp:User"];
                var smtpPass = _configuration["Smtp:Pass"];

                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUser);
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
