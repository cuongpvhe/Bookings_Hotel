using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager
{
    [Authorize(Policy = "StaffOnly")]
    public class AddNewRoomModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public List<Models.TypeRoom> RoomTypes { get; set; }

        public AddNewRoomModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RoomTypes = await _context.TypeRooms.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Lấy dữ liệu từ form
            var roomNumber = int.Parse(Request.Form["RoomNumber"]);
            var floor = roomNumber / 100;
            var roomTypeId = int.Parse(Request.Form["RoomTypeId"]);
            var description = Request.Form["Description"].ToString();

            // Lưu thông tin phòng vào bảng Room
            var room = new Models.Room
            {
                RoomNumber = roomNumber,
                Floor = floor,
                TypeId = roomTypeId,
                Description = description,
                RoomStatus = RoomStatus.ACTIVE,
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            _context.Rooms.Add(room);
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
