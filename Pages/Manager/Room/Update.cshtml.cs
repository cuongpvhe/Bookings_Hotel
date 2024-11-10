using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace Bookings_Hotel.Pages.Manager.Room
{
    [Authorize(Policy = "StaffOnly")]
    public class UpdateModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public UpdateModel(HotelBookingSystemContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Models.Room Room { get; set; }
        public List<Models.TypeRoom> RoomTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Room = await _context.Rooms
                .Include(r => r.Type)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (Room == null)
            {
                return NotFound();
            }

            RoomTypes = await _context.TypeRooms.ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var roomId = int.Parse(Request.Form["RoomId"]);
            var roomNumber = int.Parse(Request.Form["RoomNumber"]);
            var floor = roomNumber / 100;
            var roomTypeId = int.Parse(Request.Form["RoomTypeId"]);
            var roomStatus = Request.Form["RoomStatus"].ToString();
            var description = Request.Form["Description"].ToString();

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            var typeRoom = await _context.TypeRooms.FirstOrDefaultAsync(t => t.TypeId == roomTypeId);

            if (room == null || typeRoom == null)
            {
                return NotFound();
            }


            room.RoomNumber = roomNumber;
            room.Floor = floor;
            room.TypeId = roomTypeId;
            room.Description = description;
            room.RoomStatus = roomStatus;
            room.UpdateDate = DateTime.Now;
            
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostCheckRoomNumberAsync(int roomNumber)
        {
            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomNumber == roomNumber && r.RoomStatus != RoomStatus.DELETED);
            return new JsonResult(new { exists = roomExists });
        }
    }
    }
