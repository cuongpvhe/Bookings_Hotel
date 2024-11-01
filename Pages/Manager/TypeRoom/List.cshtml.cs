using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    [Authorize(Policy = "StaffOnly")]
    public class TypeRoomModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public TypeRoomModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Models.TypeRoom> TypeRooms { get;set; } = default!;
        public List<string> TableHeaders { get; set; } =
        new List<string> { "#", "Tên loại phòng", "Số giường", "Số người lớn","Số người lớn được phép thêm","Phụ phí thêm người lớn" ,"Số trẻ em","Số trẻ em được phép thêm","Phụ phí thêm trẻ em" ,"Giá phòng", "Thao tác" };

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
