using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager.Booking
{
    public class CancelledBookingsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public CancelledBookingsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<CancelledBookingDto> CancelledBookings { get; private set; } = new List<CancelledBookingDto>();

        public async Task OnGetAsync(int? roomId) // Thay đổi để roomId có thể null
        {
            if (roomId == null)
            {
                // Có thể thêm thông báo lỗi hoặc xử lý khác nếu roomId không có
                return; // Trả về mà không làm gì nếu roomId không có
            }

            // Lấy danh sách các đơn đặt phòng đã hủy từ cơ sở dữ liệu
            CancelledBookings = await _context.OrderDetails
                .Where(od => od.RoomId == roomId && od.Order.OrderStatus == OrderStatus.CANCEL)
                .Select(od => new CancelledBookingDto
                {
                    RoomNumber = od.Room.RoomNumber.ToString(),
                    UserId = od.Order.AccountId /*?? 0*/,
                    FullName = od.Order.Account.FullName,
                    CheckIn = od.CheckIn,
                    CheckOut = od.CheckOut,
                    OrderDate = od.Order.OrderDate,
                    OrderStatus = od.Order.OrderStatus.ToString()
                }).ToListAsync(); // Sử dụng ToListAsync để không chặn luồng
        }
    }

    public class CancelledBookingDto
    {
        public string RoomNumber { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public int? UserId { get; set; } 
        public string FullName { get; set; } 
    }
}
