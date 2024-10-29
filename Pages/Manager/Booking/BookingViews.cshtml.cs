using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookings_Hotel.Pages.Manager.Booking
{
    public class BookingViewsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public BookingViewsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<SelectListItem> Floors { get; set; } = new List<SelectListItem>();
        public int? SelectedFloor { get; set; }

        public void OnGet()
        {
            // Lấy danh sách tầng từ database
            Floors = _context.Rooms.Select(r => new SelectListItem
            {
                Value = r.Floor.ToString(),
                Text = $"Tầng {r.Floor}"
            }).Distinct().ToList();
        }

        // API để lấy danh sách phòng theo tầng
        public JsonResult OnGetGetRoomsByFloor(int floorId)
        {
            var rooms = _context.Rooms
                .Where(r => r.Floor == floorId)
                .Select(r => new
                {
                    roomId = r.RoomId,
                    roomNumber = r.RoomNumber
                })
                .ToList();

            return new JsonResult(rooms);
        }
        // API để lấy trạng thái booking
        public JsonResult OnGetGetBookingStatus(int roomId)
        {
            // Lấy danh sách các đơn hàng đã Confirmed
            var confirmedOrders = _context.Orders
                .Where(o => o.OrderStatus == "Confirmed")
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    OrderDetails = o.OrderDetails
                        .Where(od => od.RoomId == roomId)
                        .Select(od => new
                        {
                            CheckIn = od.CheckIn,
                            CheckOut = od.CheckOut
                        })
                        .ToList()
                })
                .ToList();

            // Lấy danh sách ngày từ hôm nay đến 30 ngày tới
            var today = DateTime.Today;
            var futureDates = Enumerable.Range(0, 30)
                .Select(offset => today.AddDays(offset))
                .ToList();

            // Tạo danh sách trạng thái cho từng ngày
            var statusList = futureDates.Select(date => new
            {
                Date = date,
                IsBooked = confirmedOrders.Any(o =>
                    o.OrderDetails.Any(od =>
                        date >= od.CheckIn && date <= od.CheckOut))
            }).ToList();

            return new JsonResult(statusList);
        }

    }
}
