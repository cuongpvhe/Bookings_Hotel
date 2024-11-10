using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static Bookings_Hotel.Pages.Manager.TypeRoom.UpdateModel;

namespace Bookings_Hotel.Pages.Manager.Services
{
    [Authorize(Policy = "StaffOnly")]
    public class CreateModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public CreateModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public Bookings_Hotel.Models.Service service { get; set; } = new Bookings_Hotel.Models.Service();


        public IActionResult OnGet()
        {
            
            return Page();
        }

        

        public async Task<IActionResult> OnPostAsync()
        {

                service.ServiceName = Request.Form["ServiceName"];
                service.Price = decimal.Parse(Request.Form["Price"]);
                service.Description = Request.Form["Description"];
                service.CreatedDate = DateTime.Now;
                service.UpdateDate = DateTime.Now;
                service.Status = ServiceStatus.ACTIVE;
            // Save service to database
                _context.Services.Add(service);
                await _context.SaveChangesAsync();

            var images = Request.Form.Files;
            for (int i = 0; i < images.Count; i++)
            {
                var file = images[i];
                var imageIndex = int.Parse(Request.Form[$"Images[{i}][index]"]);

                // New image
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "hotel_images"
                });

                var serviceImage = new ServiceImage
                {
                    ServiceId = service.ServiceId,
                    ImageUrl = uploadResult.Url.ToString(),
                    ImageIndex = imageIndex
                };

                _context.ServiceImages.Add(serviceImage);
                    

                   
                }
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            
            
        }

        public async Task<IActionResult> OnPostCheckServiceNameAsync(string serviceName)
        {
            var serviceExited = await _context.Services.AnyAsync(s => s.ServiceName == serviceName && s.Status != ServiceStatus.DELETED);
            return new JsonResult(new { exists = serviceExited });
        }
    }

    public class ImageDTO
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public int Index { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}

