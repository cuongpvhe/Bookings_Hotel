using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;

namespace Bookings_Hotel.Pages.Manager.Feedbacks
{
    public class DetailModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public DetailModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

      public Feedback Feedback { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Account)
                .Include(f => f.Order)
                .Include(f => f.Room).FirstOrDefaultAsync(m => m.ReviewId == id);
            if (feedback == null)
            {
                return NotFound();
            }
            else 
            {
                Feedback = feedback;
            }
            return Page();
        }
    }
}
