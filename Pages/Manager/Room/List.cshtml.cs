using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Bookings_Hotel.Util;

namespace Bookings_Hotel.Pages.Manager
{
    [Authorize(Policy = "StaffOnly")]
    public class RoomsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public RoomsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;


        [BindProperty]
        public List<RoomViewModel> RoomsList { get; set; }
        public List<string> TableHeaders { get; set; } =
            new List<string> { "#", "Số phòng", "Loại phòng", "Mô tả", "Trạng thái", "Thao tác" };

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var roomsQuery = _context.Rooms
                .Include(r => r.Type)
                .Where(r => r.RoomStatus != RoomStatus.DELETED)
                .AsQueryable();

            TotalPages = (int)Math.Ceiling(await roomsQuery.CountAsync() / (double)PageSize);

            RoomsList = await roomsQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(r => new RoomViewModel
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.Type != null ? r.Type.TypeName : "N/A",
                    Status = r.RoomStatus,
                    Description = r.Description
                })
                .ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnGetSearchAsync(string searchTerm, string status, int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var roomsQuery = _context.Rooms
                .Include(r => r.Type)
                .Where(r => r.RoomStatus != RoomStatus.DELETED)
                .AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                roomsQuery = roomsQuery.Where(x =>
                    x.RoomNumber.ToString().Contains(searchTerm) ||
                    x.Type.TypeName.Contains(searchTerm));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                roomsQuery = roomsQuery.Where(x => x.RoomStatus == status);
            }

            TotalPages = (int)Math.Ceiling(await roomsQuery.CountAsync() / (double)PageSize);

            RoomsList = await roomsQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(r => new RoomViewModel
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.Type != null ? r.Type.TypeName : "N/A",
                    Status = r.RoomStatus,
                    Description = r.Description
                })
                .ToListAsync();

            return Partial("PartialViews/Manager/_RoomsPartialView", this);
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            room.RoomStatus = RoomStatus.DELETED;
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
