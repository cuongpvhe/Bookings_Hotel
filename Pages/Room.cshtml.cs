using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages
{
    public class RoomModel : PageModel
    {

        private readonly HotelBookingSystemContext _context;
        public RoomModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public decimal? PriceMin { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? PriceMax { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SortPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedServices { get; set; } = new List<string>();

        public List<Room> Rooms { get; set; }
        public List<Service> Services { get; set; }

        public async Task OnGetAsync()
        {
            Services = await _context.Services.ToListAsync();

            var query = _context.Rooms.Include(x => x.Type).Include(x => x.RoomImages).Include(x => x.Reviews).AsQueryable();

            //Filter price
            if (PriceMin.HasValue)
            {
                query = query.Where(x => x.Price >= PriceMin.Value);
            }
            else if (PriceMax.HasValue)
            {
                query = query.Where(x => x.Price <= PriceMax.Value);
            }

            //Filter services
            if (SelectedServices != null && SelectedServices.Count > 0)
            {
                query = query.Where(room => room.RoomServices.Any(service => SelectedServices.Contains(service.Service.ServiceName)));
            }

            //Filter sort price
            switch (SortPrice)
            {
                case 1:
                    query = query.OrderBy(x => x.Price);
                    break;
                case 2:
                    query = query.OrderByDescending(x => x.Price);
                    break;
                default:
                    SortPrice = 3;
                    break;
            }
            Rooms = await query.ToListAsync();
        }
    }
}
