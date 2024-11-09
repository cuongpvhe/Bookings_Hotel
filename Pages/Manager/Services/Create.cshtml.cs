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
            service.CreatedDate = DateTime.Now;
            service.UpdateDate = DateTime.Now;
            service.Status = ServiceStatus.ACTIVE;
            return Page();
        }

        

        public async Task<IActionResult> OnPostAsync([FromForm] List<ImageDTO> imageDTOs)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var serviceExists = await _context.Services
                    .AnyAsync(s => s.ServiceName.ToLower() == service.ServiceName.ToLower());
                if (serviceExists)
                {
                    return new JsonResult(new
                    {
                        error = true,
                        message = "Edit error!"
                    });
                }

                service.ServiceName = Request.Form["ServiceName"];
                service.Price = decimal.Parse(Request.Form["Price"]);
                service.Description = Request.Form["Description"];
                // Save service to database
                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                foreach (var imageDTO in imageDTOs)
                {
                    if (imageDTO.ImageFile != null && imageDTO.ImageFile.Length > 0)
                    {
                        // New image
                        var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(imageDTO.ImageFile.FileName, imageDTO.ImageFile.OpenReadStream()),
                            Folder = "hotel_images"
                        });

                        var serviceImage = new ServiceImage
                        {
                            ServiceId = service.ServiceId,
                            ImageUrl = uploadResult.Url.ToString(),
                            ImageIndex = imageDTO.Index
                        };

                        _context.ServiceImages.Add(serviceImage);
                    }

                 
                    
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Service created successfully!";
                return RedirectToPage("./List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating service: " + ex.Message);
                return Page();
            }
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

