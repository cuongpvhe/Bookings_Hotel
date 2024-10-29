﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc; // Đảm bảo bạn đã thêm namespace của mô hình

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
            new List<string> { "#", "Room Number", "Room Type", "Price", "Capacity", "Status", "Actions" };

    //    public async Task OnGetAsync()
    //    {
    //        RoomsList = await _context.Rooms
    //       .Include(r => r.Type) 
    //       .Where(r => r.RoomStatus != "Deleted") 
    //       .Select(r => new RoomViewModel
    //       {
    //           RoomId = r.RoomId,
    //           RoomNumber = r.RoomNumber,
    //           RoomType = r.Type != null ? r.Type.TypeName : "N/A",
    //           Price = r.Price,
    //           Capacity = r.NumberOfBed.HasValue && r.NumberOfAdult.HasValue && r.NumberOfChild.HasValue ? $"{r.NumberOfBed} Beds, {r.NumberOfAdult} Adults, {r.NumberOfChild} Children" : "N/A",
    //           Status = r.RoomStatus,
    //           Description = r.Description
    //       })
    //       .ToListAsync();
    //    }

    //    public async Task<IActionResult> OnPostDeleteAsync(int id)
    //    {
    //        var room = await _context.Rooms.FindAsync(id);

    //        if (room == null)
    //        {
    //            return NotFound();
    //        }

    //        // Cập nhật trạng thái phòng thành "Deleted"
    //        room.RoomStatus = "Deleted";
    //        await _context.SaveChangesAsync();


    //        return new JsonResult(new
    //        {
    //            success = true,
    //            message = "Delete Success"
    //        });
    //    }

    }

    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
        public string Capacity { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
