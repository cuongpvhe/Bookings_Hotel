using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    public class ListModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public ListModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Account> Accounts { get; set; }

        public async Task OnGetAsync()
        {
            Accounts = await _context.Accounts.ToListAsync();
        }
    }
}
