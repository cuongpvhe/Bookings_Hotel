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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (RoomTypeId == 0 || ServiceIds == null || !ServiceIds.Any())
            {
                ModelState.AddModelError(string.Empty, "Please select room type and services.");
                return Page();
            }

            try
            {
                // Tạo Room mới
                var newRoom = new Models.Room
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
                    try
                    {
                        // Upload ảnh lên Cloudinary trong thư mục holtes_image
                        var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(Images[i].FileName, Images[i].OpenReadStream()),
                            Folder = "hotel_images" // Upload vào thư mục
                        });

                        // Kiểm tra nếu upload không thành công
                        if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                        {
                            var errorMessage = $"Failed to upload image {Images[i].FileName}. No URL returned.";
                            Console.WriteLine(errorMessage); // Ghi log lỗi
                            ModelState.AddModelError(string.Empty, errorMessage);
                            return Page();
                        }

                        // Lưu URL ảnh vào RoomImage
                        _context.RoomImages.Add(new RoomImage
                        {
                            RoomId = newRoom.RoomId,
                            ImageUrl = uploadResult.SecureUrl.ToString(),
                            ImageIndex = ImageIndexes[i]
                        });
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"Error uploading image {Images[i].FileName}: {ex.Message}";
                        Console.WriteLine(errorMessage); // Ghi log lỗi
                        ModelState.AddModelError(string.Empty, errorMessage);
                        return Page();
                    }
                }

                await _context.SaveChangesAsync();

                // Redirect về trang Manager/Rooms sau khi thành công
                return RedirectToPage("/Manager/Room/Rooms");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Thêm lỗi vào ModelState để hiển thị trên trang
                ModelState.AddModelError(string.Empty, "An error occurred while saving the room.");
                return Page();
            }
        }
    }
}
