using Bookings_Hotel.Common;
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

        //Attribute for filter
        [BindProperty(SupportsGet = true)]
        public decimal? PriceMin { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? PriceMax { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SortPrice { get; set; }

        //Pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public const int ItemsPerPage = 20;

        //List
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedServices { get; set; } = new List<string>();
        public List<Room> Rooms { get; set; }
        public List<Bookings_Hotel.Models.Service> Services { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Services = await _context.Services.ToListAsync();

            var query = _context.Rooms.Include(x => x.Type).Include(x => x.RoomImages).Include(x => x.Reviews).AsQueryable();

            // Filter price
            if (PriceMin.HasValue)
            {
                query = query.Where(x => x.Price >= PriceMin.Value);
            }
            if (PriceMax.HasValue)
            {
                query = query.Where(x => x.Price <= PriceMax.Value);
            }

            // Filter services
            if (SelectedServices != null && SelectedServices.Count > 0)
            {
                query = query.Where(room => room.RoomServices.Any(service => SelectedServices.Contains(service.Service.ServiceName)));
            }

            // Sort price
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

            var totalRooms = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalRooms / (double)ItemsPerPage);

            Rooms = Pagination.GetCurrentPageData(query.ToList(), CurrentPage, ItemsPerPage).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Partial("PartialViews/_RoomsPartialView", Rooms);
            }

            return Page();
        }

    }
}
