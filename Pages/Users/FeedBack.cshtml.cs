using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Users
{
    public class FeedBackModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public FeedBackModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int RoomId { get; set; }

        [BindProperty]
        public int Rating { get; set; }

        [BindProperty]
        public string Comment { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
            if (accountId == null)
            {
                return RedirectToPage("/Home/Login");
            }

            var review = new Feedback
            {
                OrderId = OrderId,
                RoomId = RoomId,
                AccountId = int.Parse(accountId),
                Rating = Rating,
                ReviewDate = DateTime.Now,
                Comment = Comment
            };

            _context.Feedbacks.Add(review);
            await _context.SaveChangesAsync();


            return RedirectToPage("/Users/HistoryBooking");
        }
    }
}
