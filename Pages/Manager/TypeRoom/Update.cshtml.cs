using Bookings_Hotel.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    [Authorize(Policy = "StaffOnly")]
    public class UpdateModel : PageModel
    {

        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public bool IsUpdate { get; set; }
        public List<SelectListItem> SelectedServices { get; set; }
        public List<TypeRoomImageDTO> ImageDTOs { get; set; }
        public Models.TypeRoom TypeRoom { get; set; }
        public List<Models.Service> Services { get; set; }

        public UpdateModel (HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                IsUpdate = false;
                Services = await _context.Services.ToListAsync();
                return Page();
            }

            // Nếu có ID, tức là ta đang trong trang cập nhật
            IsUpdate = true;

            // Lấy thông tin loại phòng
            TypeRoom = await _context.TypeRooms
                .Include(t => t.TypeRoomImages)
                .Include(t => t.TypeRoomServices)
                .FirstOrDefaultAsync(t => t.TypeId == id);

            if (TypeRoom == null)
            {
                return NotFound();
            }

            // Lấy các dịch vụ đã chọn cho loại phòng này
            var selectedServiceIds = TypeRoom.TypeRoomServices.Select(s => s.ServiceId).ToList();
            Services = await _context.Services.ToListAsync();
            SelectedServices = Services.Select(service => new SelectListItem
            {
                Value = service.ServiceId.ToString(),
                Text = service.ServiceName,
                Selected = selectedServiceIds.Contains(service.ServiceId)
            }).ToList();


            // Chuyển đổi thông tin ảnh sang DTO
            ImageDTOs = TypeRoom.TypeRoomImages.Select(img => new TypeRoomImageDTO
            {
                ImageId = img.TypeRoomImageId,
                ImageUrl = img.ImageUrl,
                Index = img.ImageIndex ?? 0
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] List<TypeRoomImageDTO> imageDTOs)
        {

            var typeId= int.Parse(Request.Form["TypeId"]);
            // Lấy loại phòng cần cập nhật
            var typeRoom = await _context.TypeRooms
                .Include(t => t.TypeRoomImages)
                .Include(t => t.TypeRoomServices)
                .FirstOrDefaultAsync(t => t.TypeId == typeId);

            if (typeRoom == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            typeRoom.TypeName = Request.Form["TypeName"];
            typeRoom.NumberOfBed = int.Parse(Request.Form["NumberOfBeds"]);

            typeRoom.NumberOfAdult = int.Parse(Request.Form["NumberOfAdults"]);
            typeRoom.MaximumExtraAdult = int.Parse(Request.Form["MaximumExtraAdult"]);
            typeRoom.ExtraAdultFee = decimal.Parse(Request.Form["ExtraAdultFee"]);

            typeRoom.NumberOfChild = int.Parse(Request.Form["NumberOfChildren"]);
            typeRoom.MaximumExtraChild = int.Parse(Request.Form["MaximumExtraChild"]);
            typeRoom.ExtraChildFee = decimal.Parse(Request.Form["ExtraChildFee"]);
            typeRoom.Price = decimal.Parse(Request.Form["Price"]);
            typeRoom.Description = Request.Form["Description"];

            // Cập nhật danh sách dịch vụ
            var selectedServiceIds = Request.Form["ServiceIds"].Select(int.Parse).ToList();
            typeRoom.TypeRoomServices.Clear();
            foreach (var serviceId in selectedServiceIds)
            {
                var typeRoomService = new Models.TypeRoomService
                {
                    TypeId = typeRoom.TypeId,
                    ServiceId = serviceId
                };
                _context.TypeRoomServices.Add(typeRoomService);
            }

            // Cập nhật ảnh
            foreach (var imageDTO in imageDTOs)
            {
                if (imageDTO.ImageId != 0)
                {
                    // Existing image
                    var existingImage = await _context.TypeRoomImages.FirstOrDefaultAsync(img => img.TypeRoomImageId == imageDTO.ImageId);
                    if (existingImage != null && imageDTO.ImageFile != null && imageDTO.ImageFile.Length > 0)
                    {
                        // Delete the old image from Cloudinary
                        var publicId = existingImage.ImageUrl.Split('/').Last().Split('.').First();
                        await _cloudinary.DestroyAsync(new DeletionParams(publicId));

                        // Upload the new image
                        var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(imageDTO.ImageFile.FileName, imageDTO.ImageFile.OpenReadStream()),
                            Folder = "hotel_images"
                        });

                        existingImage.ImageUrl = uploadResult.Url.ToString();
                        existingImage.ImageIndex = imageDTO.Index;
                    }
                }
                else if (imageDTO.ImageFile != null && imageDTO.ImageFile.Length > 0)
                {
                    // New image
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(imageDTO.ImageFile.FileName, imageDTO.ImageFile.OpenReadStream()),
                        Folder = "hotel_images"
                    });

                    var typeRoomImage = new TypeRoomImage
                    {
                        TypeId = typeRoom.TypeId,
                        ImageUrl = uploadResult.Url.ToString(),
                        ImageIndex = imageDTO.Index
                    };

                    _context.TypeRoomImages.Add(typeRoomImage);
                }
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }


        public class TypeRoomImageDTO
        {
            public int ImageId { get; set; }
            public string ImageUrl { get; set; }
            public int Index { get; set; }
            public IFormFile ImageFile { get; set; }
        }
    }
}
