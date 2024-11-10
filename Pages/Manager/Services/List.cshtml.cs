using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager.Services
{
    [Authorize(Policy = "StaffOnly")]
    public class ServicesModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public ServicesModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        // List
        public List<string>? TableHeaders { get; set; } = new List<string> { "#", "Tên dịch vụ", "Ngày tạo", "Ngày cập nhật", "Trạng thái", "Thao tác" };
        public List<Models.Service> Services { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var servicesQuery = _context.Services.Where(s => s.Status != ServiceStatus.DELETED).AsQueryable();
            TotalPages = (int)Math.Ceiling(await servicesQuery.CountAsync() / (double)PageSize);

            Services = await servicesQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            var ser = await _context.Services.FindAsync(id);
            if (ser == null)
            {
                return NotFound();
            }

            ser.Status = ser.Status == ServiceStatus.ACTIVE ? ServiceStatus.INACTIVE : ServiceStatus.ACTIVE;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm, string status, int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            IQueryable<Models.Service> servicesQuery = _context.Services.Where(s => s.Status != ServiceStatus.DELETED);

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                servicesQuery = servicesQuery.Where(x => x.ServiceName.Contains(searchTerm));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                servicesQuery = servicesQuery.Where(x => x.Status == status);
            }

            TotalPages = (int)Math.Ceiling(await servicesQuery.CountAsync() / (double)PageSize);
            Services = await servicesQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Partial("PartialViews/Manager/_ServicesPartialView", this);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var ser = await _context.Services.FindAsync(id);
            if (ser == null)
            {
                return NotFound();
            }

            ser.Status = ServiceStatus.DELETED;
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "Delete Success"
            });
        }

    }
}
