using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Manager.Services
{
    public class EditModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public EditModel(HotelBookingSystemContext context)
        {
            _context = context;
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var serviceInDb = await _context.Services.FindAsync(service.ServiceId);

            if (serviceInDb == null)
            {
                return NotFound();
            }

            // Update service fields
            serviceInDb.ServiceName = service.ServiceName;
            serviceInDb.UpdateDate = System.DateTime.Now;
            serviceInDb.Price = service.Price;
            serviceInDb.Description = service.Description;
            serviceInDb.Status = service.Status;

            await _context.SaveChangesAsync();

            return RedirectToPage("./List");
        }
    }
}
