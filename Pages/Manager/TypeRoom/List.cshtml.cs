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
    public class TypeRoomModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public TypeRoomModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Models.TypeRoom> TypeRooms { get;set; } = default!;
        public List<string> TableHeaders { get; set; } =
        new List<string> { "#", "Tên loại phòng", "Số giường", "Số người lớn", "Số trẻ em", "Giá", "Thao tác" };

        public async Task OnGetAsync()
        {
            if (_context.TypeRooms != null)
            {
                TypeRooms = await _context.TypeRooms.Where(tr => tr.Deleted == false).ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var typeRoom = await _context.TypeRooms.FindAsync(id);

            if (typeRoom == null)
            {
                return NotFound();
            }

            typeRoom.Deleted = true;
            await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                success = true,
                message = "Delete Success"
            });
        }
    }
}
