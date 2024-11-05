using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Manager
{
    [Authorize(Policy = "NonCustomerOnly")]
    public class IndexStaffModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}