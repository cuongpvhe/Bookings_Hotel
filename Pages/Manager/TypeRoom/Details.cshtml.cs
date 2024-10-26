using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    public class DetailsModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public DetailsModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

      public Models.TypeRoom TypeRoom { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.TypeRooms == null)
            {
                return NotFound();
            }

            var typeroom = await _context.TypeRooms.FirstOrDefaultAsync(m => m.TypeId == id);
            if (typeroom == null)
            {
                return NotFound();
            }
            else 
            {
                TypeRoom = typeroom;
            }
            return Page();
        }
    }
}
