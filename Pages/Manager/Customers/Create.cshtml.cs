﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookings_Hotel.Models;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager.Customers
{
    public class CreateModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public CreateModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account Account { get; set; } = new Account();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra tính duy nhất của Email
            if (await _context.Accounts.AnyAsync(a => a.Email == Account.Email))
            {
                ModelState.AddModelError("Account.Email", "Email already exists.");
                return Page();
            }

            // Kiểm tra tính duy nhất của UseName
            if (await _context.Accounts.AnyAsync(a => a.UseName == Account.UseName))
            {
                ModelState.AddModelError("Account.UseName", "Username already exists.");
                return Page();
            }
            if (await _context.Accounts.AnyAsync(a => a.FullName == Account.FullName))
            {
                ModelState.AddModelError("Account.FullName", "FullName already exists.");
                return Page();
            }
            if (await _context.Accounts.AnyAsync(a => a.Phonenumber == Account.Phonenumber))
            {
                ModelState.AddModelError("Account.Phonenumber", "Phonenumber already exists.");
                return Page();
            }
            Account.RoleId = 2;
            Account.CreatedDate = DateTime.Now;
            Account.Status = "Active";
            
            _context.Accounts.Add(Account);
            await _context.SaveChangesAsync();

            return RedirectToPage("List");
        }
    }
}
