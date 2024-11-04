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

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        public IList<Models.TypeRoom> TypeRooms { get;set; } = default!;
        public List<string> TableHeaders { get; set; } =
        new List<string> { "#", "Tên loại phòng", "Số giường", "Số người lớn","Số người lớn được phép thêm","Phụ phí thêm người lớn" ,"Số trẻ em","Số trẻ em được phép thêm","Phụ phí thêm trẻ em" ,"Giá phòng", "Thao tác" };

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var typeRoomsQuery = _context.TypeRooms
                .Where(tr => tr.Deleted == false)
                .AsQueryable();

            TotalPages = (int)Math.Ceiling(await typeRoomsQuery.CountAsync() / (double)PageSize);

            TypeRooms = await typeRoomsQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm, int pageIndex = 1)
        {
            CurrentPage = pageIndex;
            var typeRoomsQuery = _context.TypeRooms
                .Where(tr => tr.Deleted == false)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                typeRoomsQuery = typeRoomsQuery.Where(x =>
                    x.TypeName.Contains(searchTerm));
            }

            TotalPages = (int)Math.Ceiling(await typeRoomsQuery.CountAsync() / (double)PageSize);

            TypeRooms = await typeRoomsQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Partial("PartialViews/Manager/_TypeRoomsPartialView", this);
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
