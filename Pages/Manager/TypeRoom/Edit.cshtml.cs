using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    public class EditModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public EditModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.TypeRoom TypeRoom { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.TypeRooms == null)
            {
                return NotFound();
            }

            var typeroom =  await _context.TypeRooms.FirstOrDefaultAsync(m => m.TypeId == id);
            if (typeroom == null)
            {
                return NotFound();
            }
            TypeRoom = typeroom;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TypeRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeRoomExists(TypeRoom.TypeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TypeRoomExists(int id)
        {
          return (_context.TypeRooms?.Any(e => e.TypeId == id)).GetValueOrDefault();
        }
    }
}
