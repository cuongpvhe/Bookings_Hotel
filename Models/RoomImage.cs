using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class RoomImage
    {
        public int RoomImageId { get; set; }
        public string? ImageUrl { get; set; }
        public int? RoomId { get; set; }
        public int? ImageIndex { get; set; }

        public virtual Room? Room { get; set; }
    }
}
