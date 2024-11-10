using Bookings_Hotel.DTO;
using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Linq;
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
        public TypeRoomDTO typeRoomDTOGet { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //Get parameter
            if (id == null)
            {
                return NotFound();
            }

            //Process
            var typeRoom = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == id);

            if (typeRoom == null)
            {
                return NotFound("Not Found ID");
            }

            // Get Service
            var lstService = _context.TypeRoomServices
                    .Where(trs => trs.TypeId == typeRoom.TypeId)
                    .Join(_context.Services,
                        trs => trs.ServiceId,
                        s => s.ServiceId,
                        (trs, s) => s)
            .ToList();

            //Convert to ServiceDTO
            List<ServiceDTO> listServiceDTO = new List<ServiceDTO>();
            foreach (var item in lstService)
            {
                var lstImageUrl = _context.Services
                    .Where(s => s.ServiceId == item.ServiceId)
                    .Join(_context.ServiceImages,
                    s => s.ServiceId,
                    si => si.ServiceId,
                    (s, si) => si.ImageUrl)
                    .ToList();


                listServiceDTO.Add(new ServiceDTO
                {
                    ServiceId = item.ServiceId,
                    ServiceName = item.ServiceName,
                    ServiceImageUrls = lstImageUrl,
                    Description = item.Description,
                });
            }



            //Get Image
            var lstImage = _context.TypeRoomImages
                .Where(tri => tri.TypeId == typeRoom.TypeId)
                .OrderBy(tri => tri.ImageIndex)
                .Select(tri => tri.ImageUrl)
                .ToList();
            


            typeRoomDTOGet = new TypeRoomDTO
            {
                TypeId = typeRoom.TypeId,
                TypeName = typeRoom.TypeName,
                Description = typeRoom.Description,
                NumberOfChild = typeRoom.NumberOfChild,
                NumberOfAdult = typeRoom.NumberOfAdult,
                NumberOfBed = typeRoom.NumberOfBed,
                Price = typeRoom.Price,
                PriceString = typeRoom.Price.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                LstServiceObject = listServiceDTO,
                LstImage = lstImage,
            };



            return Page();


        }
    }
}