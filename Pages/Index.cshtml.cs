using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public List<TypeRoom> TypeRooms { get; set; }
        public List<TypeRoom> TypeRoomsTopRate { get; set; }
        public List<TypeRoom> TypeRoomsTopBooking { get; set; }

        public IndexModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            TypeRooms = await _context.TypeRooms.ToListAsync();

            TypeRoomsTopRate = await _context.TypeRooms.Include(tr => tr.Rooms).ThenInclude(room => room.Feedbacks)
                .Include(tr => tr.TypeRoomImages).Include(tr => tr.TypeRoomServices).ThenInclude(trs => trs.Service)
                .ThenInclude(service => service.ServiceImages)
                .Select(tr => new
                {
                    TypeRoom = tr,
                    AverageRating = tr.Rooms
                    .SelectMany(room => room.Feedbacks)
                    .Where(review => review.Rating.HasValue)
                    .Average(review => review.Rating) ?? 0
                }).OrderByDescending(tr => tr.AverageRating).Take(10).Select(tr => tr.TypeRoom).ToListAsync();

            TypeRoomsTopBooking = await _context.TypeRooms.Include(x => x.Rooms).ThenInclude(x => x.OrderDetails).Include(x => x.Rooms)
                .ThenInclude(x => x.Feedbacks).Include(x => x.TypeRoomImages).Include(x => x.TypeRoomServices).ThenInclude(y => y.Service)
                .ThenInclude(z => z.ServiceImages).Where(t => t.Rooms.Any())
                .Select(typeRoom => new
                {
                    TypeRoom = typeRoom,
                    BookingCount = typeRoom.Rooms
                    .SelectMany(r => r.OrderDetails)
                    .Count()
                }).OrderByDescending(x => x.BookingCount).Take(10).Select(x => x.TypeRoom).ToListAsync();
        }
    }
}
