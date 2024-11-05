using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Bookings_Hotel.DTO;
using System.Globalization;
using Bookings_Hotel.Models;

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
            var fromAddress = new MailAddress(smtpSettings["User"]);
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
                IsBodyHtml = true // Gửi email dưới dạng HTML
            };

            await smtp.SendMailAsync(mailMessage);
        }

        // Hàm này tạo nội dung HTML mẫu với CSS inline
        public string CreateOrderHtmlEmailContent(OrderDTO orderDTO)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            .container {{ max-width: 600px; margin: auto; padding: 20px; background-color: #f4f4f4; }}
                            .header {{ background-color: #12162c; color: white; padding: 10px 0; text-align: center; }}
                            .content {{ padding: 20px; color: #333; }}
                            .footer {{ background-color: #12162c; color: white; padding: 10px; text-align: center; font-size: 12px; }}
                            .button {{ background-color: #12162c; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Bạn đã đặt phòng thành công</h1>
                            </div>
                            <div class='content'>
                                <p>Phòng đã đặt:</p>
                                <ul>
                                    <li><b>Phòng {orderDTO.RoomType}</b></li>
                                    <dl>Ngày nhận phòng: {orderDTO.checkinDate}</dl>
                                    <dl>Ngày trả phòng: {orderDTO.checkoutDate}</dl>
                                </ul>
                                <p style=""margin-top: 35px;""><b>Ghi Chú</b>: {orderDTO.Note}</p>
                                <p><b>Số tiền đã thanh toán</b>: <b style=""font-size: 20px;"">{orderDTO.TotalMoney.ToString("N0", new CultureInfo("vi-VN"))} VNĐ </b></p>
                                <p><b>Mã số</b>: <b>#{orderDTO.OrderId}</b></p>
                                <p style=""text-align: center;"">
                                    <small>*Vui lòng không xóa email này cho đến khi bạn nhận phòng</small>
                                </p>
                            </div>
                            <div class='footer'>
                                <p>&copy; 2024 Bookings Hotel Service.</p>
                            </div>
                        </div>
                    </body>
                </html>            
                ";
        }
    }
}
