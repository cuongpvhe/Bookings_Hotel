using Bookings_Hotel.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Manager.Services
{
    public class CreateModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public CreateModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [BindProperty]
        public Bookings_Hotel.Models.Service service { get; set; } = new Bookings_Hotel.Models.Service();

        [BindProperty]
        public List<string> ImageUrls { get; set; } = new List<string>();

        public IActionResult OnGet()
        {
            service.CreatedDate = DateTime.Now;
            service.UpdateDate = DateTime.Now;
            service.Status = "Active";
            return Page();
        }

        

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Save service to database
                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                // Save images to cloud storage
                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        Folder = "hotel_images"
                    });

                    var serviceImage = new ServiceImage
                    {
                        ImageUrl = uploadResult.Url.ToString(),
                        ServiceId = service.ServiceId
                    };
                    _context.ServiceImages.Add(serviceImage);
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
}

