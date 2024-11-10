using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bookings_Hotel.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient.Server;
using Microsoft.AspNetCore.Authorization;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    [Authorize(Policy = "StaffOnly")]
    public class CreateModel : PageModel
    {

        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public List<Models.Service> Services { get; set; }

        public CreateModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Services = await _context.Services.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
         
            // Lưu thông tin phòng vào bảng Room
            var typeRoom = new Models.TypeRoom
            {
                TypeName = Request.Form["TypeName"].ToString(),
                NumberOfBed = int.Parse(Request.Form["NumberOfBeds"]),

                NumberOfAdult = int.Parse(Request.Form["NumberOfAdults"]),
                MaximumExtraAdult = int.Parse(Request.Form["MaximumExtraAdult"]),
                ExtraAdultFee = decimal.Parse(Request.Form["ExtraAdultFee"]),

                NumberOfChild = int.Parse(Request.Form["NumberOfChildren"]),
                MaximumExtraChild = int.Parse(Request.Form["MaximumExtraChild"]),
                ExtraChildFee = decimal.Parse(Request.Form["ExtraChildFee"]),
                Price = decimal.Parse(Request.Form["Price"]),
                Deleted = false,
                Description = Request.Form["Description"].ToString(),

            };
            _context.TypeRooms.Add(typeRoom);
            await _context.SaveChangesAsync();

            // Lưu danh sách dịch vụ cho phòng vào bảng RoomService
            var serviceIds = Request.Form["ServiceIds"].ToList();


            foreach (var serviceId in serviceIds)
            {
                var typeRoomService = new Models.TypeRoomService
                {
                    TypeId = typeRoom.TypeId,
                    ServiceId = int.Parse(serviceId)
                };
                _context.TypeRoomServices.Add(typeRoomService);

            }
            await _context.SaveChangesAsync(); // Lưu RoomService vào DB


            // Lưu ảnh vào bảng RoomImage
            var images = Request.Form.Files;
            for (int i = 0; i < images.Count; i++)
            {
                var file = images[i];
                var imageIndex = int.Parse(Request.Form[$"Images[{i}][index]"]);

                // Upload ảnh lên Cloudinary
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "hotel_images"
                });

                var typeRoomImage = new TypeRoomImage
                {
                    TypeId = typeRoom.TypeId,
                    ImageUrl = uploadResult.Url.ToString(),
                    ImageIndex = imageIndex
                };
                _context.TypeRoomImages.Add(typeRoomImage);
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostCheckTypeRoomNameAsync(string typeRoomName)
        {
            var typeRoomExists = await _context.TypeRooms.AnyAsync(r => r.TypeName == typeRoomName && r.Deleted != true);
            return new JsonResult(new { exists = typeRoomExists });
        }
    }
}
