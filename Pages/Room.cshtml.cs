using Bookings_Hotel.Common;
using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
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

        // Attributes for filtering
        [BindProperty(SupportsGet = true)]
        public decimal? PriceMin { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? PriceMax { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SortPrice { get; set; }

        // Pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public const int ItemsPerPage = 12;

        // Lists
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedServices { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedTypeRooms { get; set; } = new List<string>();

        [BindProperty]
        public List<Room> Rooms { get; set; }

        [BindProperty]
        public List<Bookings_Hotel.Models.Service> Services { get; set; }

        [BindProperty]
        public List<Bookings_Hotel.Models.TypeRoom> TypeRooms { get; set; }

        [BindProperty]
        public List<Bookings_Hotel.Models.Feedback> Feedbacks { get; set; }

        // New properties for search
        [BindProperty(SupportsGet = true)]
        public DateTime? CheckIn { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckOut { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AdultCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ChildCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Services = await _context.Services.Where(s => s.Status == ServiceStatus.ACTIVE).ToListAsync();

            Feedbacks = await _context.Feedbacks.Include(f => f.FeedbackImages).Include(f => f.Account)
                                                .Where(x => x.Account.Status == AccountStatus.ACTIVE)
                                                .OrderByDescending(x => x.Rating).Take(10).ToListAsync();

            TypeRooms = await _context.TypeRooms.Where(x => x.Rooms.Count > 0)
                .Include(tr => tr.Rooms)
                    .ThenInclude(r => r.OrderDetails)
                .Include(tr => tr.TypeRoomImages)
                .Include(tr => tr.TypeRoomServices)
                    .ThenInclude(ts => ts.Service)
                        .ThenInclude(s => s.ServiceImages)
                .ToListAsync();

            // Filter by CheckIn and CheckOut dates
            if (CheckIn.HasValue && CheckOut.HasValue)
            {
                foreach (var typeRoom in TypeRooms)
                {
                    typeRoom.Rooms = typeRoom.Rooms
                        .Where(r => !r.OrderDetails.Any(od =>
                            od.CheckIn < CheckOut.Value && od.CheckOut > CheckIn.Value &&
                            (od.Order != null && od.Order.OrderStatus != OrderStatus.CANCEL)
                        ))
                        .ToList();
                }
            }

            // Additional filters for each room
            if (AdultCount.HasValue)
            {
                TypeRooms = TypeRooms
                    .Where(tr => tr.NumberOfAdult >= AdultCount.Value)
                    .ToList();
            }

            if (ChildCount.HasValue)
            {
                TypeRooms = TypeRooms
                    .Where(tr => tr.NumberOfChild >= ChildCount.Value)
                    .ToList();
            }

            if (PriceMin.HasValue)
            {
                TypeRooms = TypeRooms
                    .Where(tr => tr.Price >= PriceMin.Value)
                    .ToList();
            }

            if (PriceMax.HasValue)
            {
                TypeRooms = TypeRooms
                    .Where(tr => tr.Price <= PriceMax.Value)
                    .ToList();
            }


            if (SelectedTypeRooms != null && SelectedTypeRooms.Any())
            {
                TypeRooms = TypeRooms
                    .Where(tr => SelectedTypeRooms.Contains(tr.TypeName))
                    .ToList();
            }

            if (SelectedServices != null && SelectedServices.Any())
            {
                TypeRooms = TypeRooms
                    .Where(tr => tr.TypeRoomServices.Any(ts => SelectedServices.Contains(ts.Service.ServiceName)))
                    .ToList();
            }

            // Sort by price if applicable
            TypeRooms = SortPrice switch
            {
                1 => TypeRooms.OrderBy(tr => tr.Price).ToList(),
                2 => TypeRooms.OrderByDescending(tr => tr.Price).ToList(),
                _ => TypeRooms.OrderBy(tr => tr.TypeName).ToList()
            };

            var totalRooms = TypeRooms.Count();
            TotalPages = (int)Math.Ceiling(totalRooms / (double)ItemsPerPage);

            TypeRooms = Pagination.GetCurrentPageData(TypeRooms.ToList(), CurrentPage, ItemsPerPage).ToList();

            // Return partial view for AJAX requests
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Partial("PartialViews/_RoomsPartialView", TypeRooms);

            return Page();
        }
    }
}
