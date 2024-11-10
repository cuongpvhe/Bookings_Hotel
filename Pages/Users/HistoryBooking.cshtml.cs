using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookings_Hotel.Pages.Users
{
    public class HistoryBookingModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public HistoryBookingModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public List<HistoryBooking> BookingHistory { get; set; } = new List<HistoryBooking>();

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; }

        public List<SelectListItem> StatusList => GetStatusList();

        private List<SelectListItem> GetStatusList()
        {
            return typeof(OrderStatus)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(field => new SelectListItem
                {
                    Value = field.GetValue(null)?.ToString(),
                    Text = TranslateOrderStatus(field.GetValue(null)?.ToString()) // Translate to Vietnamese
                })
                .ToList();
        }


        public async Task<IActionResult> OnGetAsync()
        {
            // Get the logged-in user's Account ID
            var accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
            if (accountId == null)
            {
                return RedirectToPage("/Home/Login");
            }

            // Build query with optional status filter
            var query = _context.Orders
                .Where(o => o.AccountId == int.Parse(accountId))
                .AsQueryable();

            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                query = query.Where(o => o.OrderStatus == SelectedStatus);
            }

            // Fetch data including RoomNumber in OrderDetailViewModel
            BookingHistory = await query
                .Select(o => new HistoryBooking
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalMoney = o.TotalMoney,
                    OrderStatus = o.OrderStatus,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        RoomId = od.RoomId ?? 0, // Default to 0 if RoomId is null
                        CheckIn = od.CheckIn,
                        CheckOut = od.CheckOut,
                        RoomNumber = od.RoomId.HasValue
                     ? _context.Rooms
                            .Where(r => r.RoomId == od.RoomId.Value)
                            .Select(r => r.RoomNumber.ToString())
                            .FirstOrDefault() ?? "Unknown"
                            : "Unknown",
                            HasFeedback = _context.Feedbacks.Any(f => f.OrderId == od.OrderId)
                    }).ToList(),
                    
                })
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Page();
        }
        // Translation helper method
        private static readonly Dictionary<string, string> OrderStatusTranslations = new()
{
    { OrderStatus.WAITING_PAYMENT, "Chờ thanh toán" },
    { OrderStatus.SUCCESS, "Thành công" },
    { OrderStatus.CANCEL, "Đã hủy" }
};

        public string TranslateOrderStatus(string status)
        {
            return OrderStatusTranslations.TryGetValue(status, out var translation) ? translation : status;
        }


        // Nested classes for history booking and order detail view model
        public class HistoryBooking
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalMoney { get; set; }
            public string OrderStatus { get; set; }
            public List<OrderDetailViewModel> OrderDetails { get; set; }
           
        }

        public class OrderDetailViewModel
        {
            public int RoomId { get; set; }
            public string RoomNumber { get; set; }
            public DateTime CheckIn { get; set; }
            public DateTime CheckOut { get; set; }
            public bool HasFeedback { get; set; }
        }
    }
}
