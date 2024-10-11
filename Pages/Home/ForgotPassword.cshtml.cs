using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net;
using System.Net.Mail;

namespace Bookings_Hotel.Pages.Home
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        public string Message { get; set; }

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

            // Tạo mã OTP
            int otp = GenerateOTP();
            TempData["OTP"] = otp; // Lưu OTP vào TempData

            // Gửi OTP qua email
            if (SendOTPForgotPass(Email, otp))
            {
                TempData["Account_ID"] = GetUserIdByEmail(Email); // Lưu UserID
                Message = "Mã OTP đã được gửi đến email của bạn.";
                return RedirectToPage("VerifyOTPForgotPassword"); // Chuyển đến trang xác thực OTP
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
                // Cấu hình SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("phamduycuong2k1@gmail.com", "krowpvhmjfsndfum");
                smtpClient.EnableSsl = true;

                // Soạn thảo email
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("phamduycuong2k1@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Mã OTP để đặt lại mật khẩu";
                mailMessage.Body = "Mã OTP của bạn là: " + otp.ToString();

                // Gửi email
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
            // Thực hiện truy vấn để lấy UserID từ email
            // Trả về UserID tương ứng
            return 1; // Thay đổi để lấy giá trị thực tế
        }
    }
}
