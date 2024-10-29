using Bookings_Hotel.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager
{
    public class AddNewRoomModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public List<TypeRoom> RoomTypes { get; set; }
        public List<Models.Service> Services { get; set; }

        public AddNewRoomModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RoomTypes = await _context.TypeRooms.ToListAsync();
            Services = await _context.Services.ToListAsync();
            return Page();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    // Lấy dữ liệu từ form
        //    var roomNumber = int.Parse(Request.Form["RoomNumber"]);
        //    var numberOfBeds = int.Parse(Request.Form["NumberOfBeds"]);
        //    var numberOfAdults = int.Parse(Request.Form["NumberOfAdults"]);
        //    var numberOfChildren = int.Parse(Request.Form["NumberOfChildren"]);
        //    var price = decimal.Parse(Request.Form["Price"]);
        //    var roomTypeId = int.Parse(Request.Form["RoomTypeId"]);
        //    var description = Request.Form["Description"].ToString();

        //    // Lưu thông tin phòng vào bảng Room
        //    var room = new Models.Room
        //    {
        //        RoomNumber = roomNumber,
        //        NumberOfBed = numberOfBeds,
        //        NumberOfAdult = numberOfAdults,
        //        NumberOfChild = numberOfChildren,
        //        Price = price,
        //        TypeId = roomTypeId,
        //        Description = description,
        //        RoomStatus = "Active",
        //        CreatedDate = DateTime.Now,
        //        UpdateDate = DateTime.Now,
        //    };
        //    _context.Rooms.Add(room);
        //    await _context.SaveChangesAsync();

        //    // Lưu danh sách dịch vụ cho phòng vào bảng RoomService
        //    var serviceIds = Request.Form["ServiceIds"].ToList();


        //    foreach (var serviceId in serviceIds)
        //        {
        //            var roomService = new Models.RoomService
        //            {
        //                RoomId = room.RoomId,
        //                ServiceId = int.Parse(serviceId)
        //            };
        //            _context.RoomServices.Add(roomService);
                    
        //        }
        //        await _context.SaveChangesAsync(); // Lưu RoomService vào DB


        //    // Lưu ảnh vào bảng RoomImage
        //    var images = Request.Form.Files;
        //    for (int i = 0; i < images.Count; i++)
        //    {
        //        var file = images[i];
        //        var imageIndex = int.Parse(Request.Form[$"Images[{i}][index]"]);

        //        // Upload ảnh lên Cloudinary
        //        var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
        //        {
        //            File = new FileDescription(file.FileName, file.OpenReadStream()),
        //            Folder = "hotel_images"
        //        });

        //        var roomImage = new RoomImage
        //        {
        //            RoomId = room.RoomId,
        //            ImageUrl = uploadResult.Url.ToString(),
        //            ImageIndex = imageIndex
        //        };
        //        _context.RoomImages.Add(roomImage);
        //    }

        //    await _context.SaveChangesAsync();

        //    return new JsonResult(new { success = true });
        //}

        //public async Task<IActionResult> OnPostCheckRoomNumberAsync(int roomNumber)
        //{
        //    var roomExists = await _context.Rooms.AnyAsync(r => r.RoomNumber == roomNumber);
        //    return new JsonResult(new { exists = roomExists });
        //}
    }



}
