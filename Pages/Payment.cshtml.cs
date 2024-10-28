using Bookings_Hotel.DTO;
using Bookings_Hotel.Models;
using Bookings_Hotel.Service;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Claims;

namespace Bookings_Hotel.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public PaymentModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public PaymentDTO paymentDTO { get; set; }

        public async Task<IActionResult> OnGetAsync(int? oid)
        {
            //Get parameter
            if (oid == null)
            {
                return NotFound();
            }

            //Check Login
            var accountId = User.FindFirstValue("AccountId"); 

            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized();
            }

            var account = await _context.Accounts.FindAsync(int.Parse(accountId));
            if (account == null)
            {
                return Unauthorized();
            }

            
            //Process
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == oid && o.AccountId == Int64.Parse(accountId));
            
            //Check is order is payed
            if (order == null || order.OrderStatus.Equals(OrderStatus.WAITING_CONFIRM))
            {
                return NotFound();
            }

            if (order == null)
            {
                return NotFound();
            }

            //Check if the order wasn't placed 30 minutes ago
            var timeSinceOrderPlaced = DateTime.Now - order.OrderDate;
            if (timeSinceOrderPlaced.TotalMinutes > 30)
            {
                // Update the order status to "Canceled"
                order.OrderStatus = OrderStatus.CANCEL;

                // Save the changes to the database
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                // Return NotFound
                return NotFound();
            }

            //Create payment
            paymentDTO = new PaymentDTO
            (
                order.OrderId,
                order.TotalMoney,
                order.PaymentCode,
                order.TotalMoney.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
            ); 

            return Page();
        }

        public async Task<IActionResult> OnPostCheckPayment(int orderID)
        {
            // Kiểm tra trạng thái giao dịch của order
            var order = await _context.Orders.FindAsync(orderID);
            if (order == null)
            {
                return new JsonResult(new { success = false, message = "Order not found." });
            }

            // Lấy toàn bộ giao dịch
            string jsonData = await CassoIntegration.GetTransactionsAsync();


           PaymentDTO payment = new PaymentDTO
            (
                order.OrderId,
                order.TotalMoney,
                order.PaymentCode,
                order.TotalMoney.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
            );

            bool isPaymented = CheckTransactions(jsonData, payment);

            if (isPaymented)
            {
                order.OrderStatus = OrderStatus.WAITING_CONFIRM;

                // Save the changes to the database
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return new JsonResult(new
                {
                    success = true,
                    message = "Payment Success"
                });
            }
            
            
            return new JsonResult(new { 
                success = false, 
                message = "Payment Fail"
            });
            
        }

        public static bool CheckTransactions(string jsonData, PaymentDTO payment)
        {
            // Parse JSON data
            var parsedJson = JObject.Parse(jsonData);

            List<TransactionRecordDTO> transactionRecords = new List<TransactionRecordDTO>();

            List<JObject> records = parsedJson["data"]["records"].ToObject<List<JObject>>();

            foreach (var record in records)
            {
                TransactionRecordDTO transaction = record.ToObject<TransactionRecordDTO>();
                transactionRecords.Add(transaction);
            }

            foreach (var transaction in transactionRecords)
            {
                if(transaction.Amount == payment.Money && transaction.Description.IndexOf(payment.CurrencyCode, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            // Trả về false chỉ để làm mẫu, bạn có thể thay đổi logic kiểm tra ở đây
            return false;
        }
    }
}
