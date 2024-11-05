using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    [Authorize(Policy = "StaffOnly")]
    public class ListModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public ListModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Account> Accounts { get; set; }
        public string SearchString { get; set; }
        public async Task OnGetAsync(string searchString)
        {
            SearchString = searchString;

            // Start with accounts filtered by role
            var accountsQuery = _context.Accounts.Where(account => account.RoleId == 2);

            // Execute the query to retrieve accounts from the database
            var accounts = await accountsQuery.ToListAsync();

            // If a search string is provided, filter accounts based on multiple fields in memory
            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(account =>
            (account.FullName != null && account.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||    // Search by full name
            (account.Email != null && account.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||       // Search by email
            (account.Gender != null && account.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||      // Search by gender
            (account.Status != null && account.Status.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||      // Search by status
            (account.Address != null && account.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||      // Search by address
            (account.Dob.HasValue && account.Dob.Value.ToString("yyyy-MM-dd").Contains(searchString)) || // Check for Dob
            (account.Phonenumber != null && account.Phonenumber.Contains(searchString)) // Search by phone number
        ).ToList();
            }

            // Assign the filtered results to the Accounts property
            Accounts = accounts;
        }


        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            var customer = await _context.Accounts.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Status = customer.Status == "Active" ? "Inactive" : "Active";
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var customer = await _context.Accounts.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(customer);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
