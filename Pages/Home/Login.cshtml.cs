using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings_Hotel.Pages.Home
{
    public class LoginModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public LoginModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
           
            var account = _context.Accounts.FirstOrDefault(a => a.UseName == Username.Trim() && a.Password == Password);

            if (account != null)
            {
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.UseName),
                    new Claim("AccountId", account.AccountId.ToString()),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Role, account.RoleId.ToString())
                };

               
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

             
                if (account.RoleId == 2) 
                {
                    return RedirectToPage("/Index");
                }
                else if (account.RoleId == 1)
                {
                    return RedirectToPage("/Admin/Managers");
                }
                else if (account.RoleId == 3) 
                {
                    return RedirectToPage("/Staff/Managers");
                }
            }

            
            ModelState.AddModelError("Error_Login", "Invalid username or password.");
            return Page();
        }
    }
}
