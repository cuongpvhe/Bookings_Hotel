using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Bookings_Hotel.Pages.Manager
{
    public class RoomsModel : PageModel
    {
        public List<string>? TableHeaders { get; set; }
        public List<Room>? RoomsList { get; set; }

        public void OnGet()
        {
            // Khởi tạo tiêu đề và danh sách phòng
            TableHeaders = new List<string> { "#", "Room Id", "Room Number", "Room Type", "Price", "Capacity", "Status", "Actions" };
            RoomsList = new List<Room>
        {
            new Room { Id = 1, RoomId = 101, RoomNumber = "A101", RoomType = "Deluxe", Price = 500000, Capacity = 2, Status = "Available" },
            new Room { Id = 2, RoomId = 102, RoomNumber = "A102", RoomType = "Standard", Price = 300000, Capacity = 2, Status = "Booked" }
        };
        }

        public class Room
        {
            public int? Id { get; set; }
            public int? RoomId { get; set; }
            public string? RoomNumber { get; set; }
            public string? RoomType { get; set; }
            public decimal? Price { get; set; }
            public int? Capacity { get; set; }
            public string? Status { get; set; }
        }
    }
}
