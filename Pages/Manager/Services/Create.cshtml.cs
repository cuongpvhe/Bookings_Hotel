using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Manager.Services
{
    public class CreateModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public CreateModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Bookings_Hotel.Models.Service service { get; set; } = new Bookings_Hotel.Models.Service();

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
                _context.Services.Add(service);
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

