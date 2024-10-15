using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public List<Room> Rooms { get; set; }

        public IndexModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Rooms = _context.Rooms.Include(x => x.Type).Include(x => x.RoomImages).Include(x => x.Reviews).ToList();
        }
    }
}
