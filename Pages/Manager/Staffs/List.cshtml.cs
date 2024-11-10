using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager.Staffs
{
    public class ListModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public ListModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        //List
        public List<string>? TableHeaders { get; set; } = new List<string> { ".No", "Email", "Họ và tên", "Số điện thoại", "Ngày sinh", "Giới tính", "Trạng thái", "Thao tác" };
        public List<Account> Staffs { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var staffsQuery = _context.Accounts.Where(x => x.Role.RoleName == RoleName.STAFF).AsQueryable();
            TotalPages = (int)Math.Ceiling(await staffsQuery.CountAsync() / (double)PageSize);

            Staffs = await staffsQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm, string status, int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var staffsQuery = _context.Accounts.Where(x => x.Role.RoleName == RoleName.STAFF);

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                staffsQuery = staffsQuery.Where(x => x.Email.Contains(searchTerm) || x.FullName.Contains(searchTerm));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                staffsQuery = staffsQuery.Where(x => x.Status == status);
            }

            TotalPages = (int)Math.Ceiling(await staffsQuery.CountAsync() / (double)PageSize);
            Staffs = await staffsQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Partial("PartialViews/Manager/_StaffsPartialView", this);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
