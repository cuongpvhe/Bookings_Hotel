using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class RoomService
    {
        public int RoomServiceId { get; set; }
        public int? RoomId { get; set; }
        public int? ServiceId { get; set; }

        public virtual Room? Room { get; set; }
        public virtual Service? Service { get; set; }
    }
}
