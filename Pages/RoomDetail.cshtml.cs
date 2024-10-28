using Bookings_Hotel.DTO;
using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Claims;

namespace Bookings_Hotel.Pages
{
    public class RoomDetailModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public RoomDetailModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public TypeRoomDTO roomDTOGet { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //Get parameter
            if (id == null)
            {
                return NotFound();
            }

            //Process
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }


           /* roomDTOGet = new RoomDTO(
                room.RoomId,
                room.RoomNumber,
                room.NumberOfChild,
                room.NumberOfAdult,
                room.NumberOfBed,
                room.Price,
                room.Price.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                room.Description
            );*/



            return Page();


        }
    }
}