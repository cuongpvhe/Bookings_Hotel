using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Bookings_Hotel.Util; // Đảm bảo bạn đã thêm namespace của mô hình

namespace Bookings_Hotel.Pages.Manager
{
    public class RoomsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public RoomsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }
        [BindProperty]
        public List<RoomViewModel> RoomsList { get; set; }
        public List<string> TableHeaders { get; set; } = 
            new List<string> { "#", "Số phòng", "Loại phòng", "Mô tả", "Trạng thái" , "Thao tác" };

        public async Task OnGetAsync()
        {
            RoomsList = await _context.Rooms
           .Include(r => r.Type) 
           .Where(r => r.RoomStatus != "Deleted") 
           .Select(r => new RoomViewModel
           {
               RoomId = r.RoomId,
               RoomNumber = r.RoomNumber,
               RoomType = r.Type != null ? r.Type.TypeName : "N/A",
               Status = r.RoomStatus,
               Description = r.Description
           })
           .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            room.RoomStatus = "Deleted";
            await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                success = true,
                message = "Delete Success"
            });
        }

        public async Task<IActionResult> OnPostActiveAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            room.RoomStatus = RoomStatus.ACTIVE;
            await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                success = true,
                message = "Active Success"
            });
        }

        public async Task<IActionResult> OnPostDeactiveAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            room.RoomStatus = RoomStatus.INACTIVE;
            await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                success = true,
                message = "Deactive Success"
            });
        }

    }

    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
