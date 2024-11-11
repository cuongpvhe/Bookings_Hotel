using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public IndexModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int NumberRoom { get; set; }

        [BindProperty]
        public int NumberService { get; set; }

        [BindProperty]
        public int NumberCustomer { get; set; }

        [BindProperty]
        public List<TypeRoom> TypeRooms { get; set; }

        [BindProperty]
        public List<TypeRoom> TypeRoomsTopRate { get; set; }

        [BindProperty]
        public List<TypeRoom> TypeRoomsTopBooking { get; set; }

        [BindProperty]
        public List<Bookings_Hotel.Models.Service> Services { get; set; }

        [BindProperty]
        public List<Bookings_Hotel.Models.Feedback> Feedbacks { get; set; }

        public async Task OnGetAsync()
        {
            NumberRoom = _context.Rooms.ToList().Count;
            NumberService = _context.Services.ToList().Count;
            NumberCustomer = _context.Accounts.Where(a => a.Role.RoleName == RoleName.CUSTOMER).ToList().Count;

            TypeRooms = await _context.TypeRooms.Where(x => x.Rooms.Count > 0).ToListAsync();

            TypeRoomsTopRate = await _context.TypeRooms.Where(x => x.Rooms.Count > 0).Include(tr => tr.Rooms).ThenInclude(room => room.Feedbacks)
                .Include(tr => tr.TypeRoomImages).Include(tr => tr.TypeRoomServices).ThenInclude(trs => trs.Service)
                .ThenInclude(service => service.ServiceImages)
                .Select(tr => new
                {
                    TypeRoom = tr,
                    AverageRating = tr.Rooms
                    .SelectMany(room => room.Feedbacks)
                    .Where(review => review.Rating.HasValue)
                    .Average(review => review.Rating) ?? 0
                }).OrderByDescending(tr => tr.AverageRating).Take(12).Select(tr => tr.TypeRoom).ToListAsync();

            TypeRoomsTopBooking = await _context.TypeRooms.Where(x => x.Rooms.Count > 0).Include(x => x.Rooms).ThenInclude(x => x.OrderDetails).Include(x => x.Rooms)
                .ThenInclude(x => x.Feedbacks).Include(x => x.TypeRoomImages).Include(x => x.TypeRoomServices).ThenInclude(y => y.Service)
                .ThenInclude(z => z.ServiceImages).Where(t => t.Rooms.Any())
                .Select(typeRoom => new
                {
                    TypeRoom = typeRoom,
                    BookingCount = typeRoom.Rooms
                    .SelectMany(r => r.OrderDetails)
                    .Count()
                }).OrderByDescending(x => x.BookingCount).Take(12).Select(x => x.TypeRoom).ToListAsync();

            Services = await _context.Services.Include(x => x.ServiceImages).Where(x => x.Status == ServiceStatus.ACTIVE).ToListAsync();

            Feedbacks = await _context.Feedbacks.Include(f => f.FeedbackImages).Include(f => f.Account)
                                                .Where(x => x.Account.Status == AccountStatus.ACTIVE)
                                                .OrderByDescending(x => x.Rating).Take(10).ToListAsync();
        }
    }
}
