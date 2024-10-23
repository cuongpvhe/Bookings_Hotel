using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager
{
    public class ServicesModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public ServicesModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        // List
        public List<string>? TableHeaders { get; set; } = new List<string> { ".No", "Service Id", "Service Name", "Created Date", "Update Date", "Price", "Actions" };
        public List<Service> Services { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 20;

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var servicesQuery = _context.Services.AsQueryable();
            TotalPages = (int)Math.Ceiling(await servicesQuery.CountAsync() / (double)PageSize);

            Services = await servicesQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm, int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            IQueryable<Service> servicesQuery;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                servicesQuery = _context.Services;
            }
            else
            {
                servicesQuery = _context.Services.Where(x => x.ServiceName.Contains(searchTerm));
            }

            TotalPages = (int)Math.Ceiling(await servicesQuery.CountAsync() / (double)PageSize);

            Services = await servicesQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            return Partial("PartialViews/Manager/_ServicesPartialView", this);
        }
    }

}
