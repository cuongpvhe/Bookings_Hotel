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

        public AddNewRoomModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [BindProperty]
        public Room NewRoom { get; set; }

        [BindProperty]
        public List<int> SelectedServiceIds { get; set; } // Holds selected service IDs

        [BindProperty]
        public IFormFileCollection Images { get; set; } // Holds uploaded images

        public List<TypeRoom> TypeRooms { get; set; }
        public List<Service> Services { get; set; }

        public async Task OnGetAsync()
        {
            TypeRooms = await _context.TypeRooms.ToListAsync();
            Services = await _context.Services.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || SelectedServiceIds == null || !SelectedServiceIds.Any())
            {
                // Re-populate dropdowns if there's an error
                TypeRooms = await _context.TypeRooms.ToListAsync();
                Services = await _context.Services.ToListAsync();
                ModelState.AddModelError("SelectedServiceIds", "At least one service must be selected.");
                return Page();
            }

            // Set default RoomStatus to "Available"
            NewRoom.RoomStatus = "Available";
            NewRoom.CreatedDate = DateTime.Now;
            NewRoom.UpdateDate = DateTime.Now;

            // Add the new room to the database
            _context.Rooms.Add(NewRoom);
            await _context.SaveChangesAsync();

            // Add RoomServices for each selected service
            foreach (var serviceId in SelectedServiceIds)
            {
                var roomService = new RoomService
                {
                    RoomId = NewRoom.RoomId,
                    ServiceId = serviceId
                };
                _context.RoomServices.Add(roomService);
            }

            // Upload images to Cloudinary
            var roomImages = new List<RoomImage>();
            foreach (var image in Images)
            {
                var uploadResult = await UploadImageToCloudinary(image);
                if (uploadResult != null)
                {
                    roomImages.Add(new RoomImage
                    {
                        RoomId = NewRoom.RoomId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri
                    });
                }
            }

            _context.RoomImages.AddRange(roomImages);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Room created successfully!";
            return RedirectToPage();
        }

        private async Task<ImageUploadResult> UploadImageToCloudinary(IFormFile image)
        {
            using var stream = image.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                Folder = "hotel_images", // Cloudinary folder
                Transformation = new Transformation().Width(1000).Height(600).Crop("fit")
            };
            return await _cloudinary.UploadAsync(uploadParams);
        }
    }

}
