﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;

namespace Bookings_Hotel.Pages.Manager.TypeRoom
{
    public class TypeRoomModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        public TypeRoomModel(Bookings_Hotel.Models.HotelBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Models.TypeRoom> TypeRooms { get;set; } = default!;
        public List<string> TableHeaders { get; set; } =
        new List<string> { "#", "Type Name", "Number Of Bed", "Number Of Adult", "Number Of Child", "Price", "Action" };

        public async Task OnGetAsync()
        {
            if (_context.TypeRooms != null)
            {
                TypeRooms = await _context.TypeRooms.ToListAsync();
            }
        }
    }
}
