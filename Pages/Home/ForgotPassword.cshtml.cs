using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;

namespace Bookings_Hotel.Pages.Home
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public string Email { get; set; }

        public string Message { get; set; }

        public ForgotPasswordModel(HotelBookingSystemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError("Email", "Vui lòng nhập địa chỉ email.");
                return Page();
            }
            var account = _context.Accounts.FirstOrDefault(a => a.Email == Email);
            if (account == null)
            {
                ModelState.AddModelError("Email", "Email chưa được đăng ký.");
                return Page(); // Return to the page with the error message
            }

            // Tạo mã OTP
            int otp = GenerateOTP();
            TempData["OTP"] = otp; // Lưu OTP vào TempData

            // Gửi OTP qua email
            if (SendOTPForgotPass(Email, otp))
            {
                TempData["Account_ID"] = GetUserIdByEmail(Email); // Lưu UserID
                Message = "Mã OTP đã được gửi đến email của bạn.";
                return RedirectToPage("/Home/VerifyOTPForgotPassword"); // Chuyển đến trang xác thực OTP
            }
            else
            {
                ModelState.AddModelError("", "Gửi mã OTP thất bại. Vui lòng thử lại.");
                return Page();
            }
        }

        private int GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }

        private bool SendOTPForgotPass(string email, int otp)
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

        private int GetUserIdByEmail(string email)
        {
            // Tìm tài khoản trong cơ sở dữ liệu theo email
            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);
            return account?.AccountId ?? -1; // Trả về -1 nếu không tìm thấy tài khoản
        }

    }
}
