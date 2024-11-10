using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Bookings_Hotel.Util;

namespace Bookings_Hotel.Pages.Manager.Feedbacks
{
    public class ListModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public ListModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

        //List
        public List<string>? TableHeaders { get; set; } = new List<string> { ".No", "Email", "Phản hồi", "Đánh giá", "Ngày phản hồi", "Thao tác" };

        public IList<Feedback> Feedbacks { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 20;

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var feedbacks = _context.Feedbacks
                .Include(f => f.Account)
                .Include(f => f.Order)
                .Include(f => f.Room).AsQueryable();

            TotalPages = (int)Math.Ceiling(await feedbacks.CountAsync() / (double)PageSize);

            Feedbacks = await feedbacks.Skip((CurrentPage - 1) * PageSize)
                                       .Take(PageSize)
                                       .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm, int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var feedbacks = _context.Feedbacks
                .Include(f => f.Account)
                .Include(f => f.Order)
                .Include(f => f.Room).AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                feedbacks = feedbacks.Where(x => x.Account.Email.Contains(searchTerm) || x.Account.FullName.Contains(searchTerm));
            }

            TotalPages = (int)Math.Ceiling(await feedbacks.CountAsync() / (double)PageSize);
            Feedbacks = await feedbacks
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Partial("PartialViews/Manager/_FeedbacksPartialView", this);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
