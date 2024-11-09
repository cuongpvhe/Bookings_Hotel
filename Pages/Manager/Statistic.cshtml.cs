using Bookings_Hotel.Models;
using Bookings_Hotel.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager
{
    [Authorize(Policy = "ManagerOnly")]
    public class StatisticModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public StatisticModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetRevenueByYear(int year)
        {
            // Truy vấn tổng doanh thu cho từng tháng trong năm đã chọn
            var monthlyRevenue = _context.Orders
                .Where(o => o.OrderDate.Year == year)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalMoney) // Tổng tiền của các đơn hàng trong tháng
                })
                .ToList();

            // Đảm bảo trả về dữ liệu đầy đủ 12 tháng, nếu thiếu tháng nào sẽ đặt giá trị là 0
            var revenueData = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    Month = month,
                    TotalRevenue = monthlyRevenue.FirstOrDefault(r => r.Month == month)?.TotalRevenue ?? 0
                })
                .ToList();

            return new JsonResult(revenueData);
        }

        public async Task<JsonResult> OnGetRoomTypeBookingByMonth(int year, int month)
        {
            // Truy vấn để lấy số lượng loại phòng được đặt trong tháng và năm đã chọn
            var roomTypeBookings = await _context.Orders
                .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == month) // Lọc theo năm và tháng
                .Join(
                    _context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { o, od }
                )
                .Join(
                    _context.Rooms,
                    combined => combined.od.RoomId,
                    r => r.RoomId,
                    (combined, r) => new { combined.o, combined.od, r }
                )
                .Join(
                    _context.TypeRooms,
                    combined => combined.r.TypeId,
                    tr => tr.TypeId,
                    (combined, tr) => new { combined.o, tr }
                )
                .GroupBy(
                    x => x.tr.TypeName // Nhóm theo loại phòng
                )
                .Select(g => new
                {
                    RoomType = g.Key,
                    BookedRoomCount = g.Count() // Đếm số lượng đặt phòng của từng loại phòng
                })
                .OrderBy(result => result.RoomType) // Sắp xếp theo tên loại phòng
                .ToListAsync();

            return new JsonResult(roomTypeBookings);
        }

    }
}
