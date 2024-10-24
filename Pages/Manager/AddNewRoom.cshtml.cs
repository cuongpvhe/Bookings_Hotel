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
        public List<Service> Services { get; set; }

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

        public async Task<IActionResult> OnPostAsync(List<IFormFile> Images, List<int> ImageIndexes, int RoomTypeId, List<int> ServiceIds)
        {
            // Kiểm tra dữ liệu từ form có đầy đủ không
            if (!ModelState.IsValid)
            {
                // Nếu không hợp lệ, trả về trang hiện tại
                return Page();
            }

            // Kiểm tra nếu RoomTypeId hoặc ServiceIds bị null hoặc rỗng
            if (RoomTypeId == 0 || ServiceIds == null || !ServiceIds.Any())
            {
                ModelState.AddModelError(string.Empty, "Please select room type and services.");
                return Page();
            }

            // Tiếp tục với logic lưu dữ liệu
            try
            {
                // Tạo Room mới
                var newRoom = new Room
                {
                    RoomNumber = int.Parse(Request.Form["RoomNumber"]),
                    NumberOfBed = int.Parse(Request.Form["NumberOfBeds"]),
                    NumberOfAdult = int.Parse(Request.Form["NumberOfAdults"]),
                    NumberOfChild = int.Parse(Request.Form["NumberOfChildren"]),
                    Price = decimal.Parse(Request.Form["Price"]),
                    TypeId = RoomTypeId,
                    Description = Request.Form["Description"],
                    CreatedDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    RoomStatus = "Available",
                };

                _context.Rooms.Add(newRoom);
                await _context.SaveChangesAsync();

                // Lưu RoomService
                foreach (var serviceId in ServiceIds)
                {
                    _context.RoomServices.Add(new RoomService
                    {
                        RoomId = newRoom.RoomId,
                        ServiceId = serviceId
                    });
                }

                // Xử lý upload ảnh
                for (int i = 0; i < Images.Count; i++)
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(Images[i].FileName, Images[i].OpenReadStream())
                    });

                    _context.RoomImages.Add(new RoomImage
                    {
                        RoomId = newRoom.RoomId,
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        ImageIndex = ImageIndexes[i]
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("/Manager/Rooms");
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine(ex.Message);

                // Thêm thông báo lỗi vào ModelState để hiển thị trong trang
                ModelState.AddModelError(string.Empty, "An error occurred while saving the room.");
                return Page();
            }
        }
    }
}
