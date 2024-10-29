using System.Net.Mail;
using System.Net;

namespace Bookings_Hotel.Service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string content)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            var fromAddress = new MailAddress(smtpSettings["User"]); // Sử dụng `User` làm địa chỉ người gửi
            var toAddress = new MailAddress(email);

            using var smtp = new SmtpClient
            {
                Host = smtpSettings["Server"],
                Port = int.Parse(smtpSettings["Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpSettings["User"], smtpSettings["Pass"])
            };

            using var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true // Nếu cần gửi HTML email, để `true`
            };

            await smtp.SendMailAsync(mailMessage);
        }
    }
}
