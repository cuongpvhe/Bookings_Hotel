using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Bookings_Hotel.Pages.Home
{
    public class LogOutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Xóa tất cả session
            HttpContext.Session.Clear();

          
            // Chuyển hướng về trang Home
            return RedirectToPage("/Index");
        }
    }
}
