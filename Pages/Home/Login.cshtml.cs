using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Home
{
    public class LoginModel : PageModel
    {
        private readonly Booking_hotelContext _context;

        public LoginModel(Booking_hotelContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == Username.Trim() && u.Password == Password);
            var manager = _context.Managers.FirstOrDefault(u => u.Username == Username.Trim() && u.Password == Password);

            if (user != null)
            {
                HttpContext.Session.SetString("user", user.UserId.ToString());
                return RedirectToPage("Index");
            }
            else if (manager != null)
            {
                if (manager.Role == 1)
                {
                    HttpContext.Session.SetString("admin", manager.ManagerId.ToString());
                    return RedirectToPage("Admin/Managers");
                }
                else if (manager.Role == 2)
                {
                    HttpContext.Session.SetString("staff", manager.ManagerId.ToString());
                    return RedirectToPage("Staff/Managers");
                }
            }

            ModelState.AddModelError("Error_Login", "Invalid username or password.");
            return Page();
        }
    }
}
