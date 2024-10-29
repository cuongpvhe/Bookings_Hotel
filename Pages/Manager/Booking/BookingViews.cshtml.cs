
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bookings_Hotel.Pages.Manager.Bookings
{
    public class BookingViewsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public BookingViewsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int SelectedFloor { get; set; }

        public List<int> Floors { get; set; }
        public List<Room> Rooms { get; set; }
        public List<OrderDetail> RoomBookings { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy danh sách các tầng
            Floors = await _context.Rooms
                .Where(r => r.Floor.HasValue)
                .Select(r => r.Floor.Value)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            // Lấy danh sách các phòng thuộc tầng đã chọn
            Rooms = await _context.Rooms
                .Where(r => r.Floor == SelectedFloor)
                .ToListAsync();
        }

        public async Task<JsonResult> OnGetRoomBookingsAsync(int roomId, int month, int year)
        {
            // Lấy các booking cho phòng và tháng, năm đã chọn
            var bookings = await _context.OrderDetails
                .Where(od => od.RoomId == roomId &&
                             od.CheckIn.Month == month &&
                             od.CheckIn.Year == year)
                .Include(od => od.Order) // Bao gồm thông tin của Order để lấy Account
                .ToListAsync();

            return new JsonResult(bookings);
        }
    }
}
