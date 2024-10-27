using Bookings_Hotel.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Manager.Services
{
    public class EditModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;

        public EditModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [BindProperty]
        public Bookings_Hotel.Models.Service service { get; set; } = new Bookings_Hotel.Models.Service();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Edit failure!";
                    return Page();
                }

                var serviceInDb = await _context.Services.FindAsync(service.ServiceId);

                if (serviceInDb == null)
                {
                    TempData["ErrorMessage"] = "Edit failure!";
                    return NotFound();
                }

                serviceInDb.ServiceName = service.ServiceName;
                serviceInDb.UpdateDate = System.DateTime.Now;
                serviceInDb.Price = service.Price;
                serviceInDb.Description = service.Description;
                serviceInDb.Status = service.Status;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Edit successfully";

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToPage("./Edit", new { id = service.ServiceId });
        }
    }
}
