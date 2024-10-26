using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bookings_Hotel.Models;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    public class CreateModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public CreateModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.TypeRoom TypeRoom { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.TypeRooms == null || TypeRoom == null)
            {
                return Page();
            }

            _context.TypeRooms.Add(TypeRoom);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
