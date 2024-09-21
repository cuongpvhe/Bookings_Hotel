using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int ManagerId { get; set; }
        public decimal? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? ReviewDate { get; set; }

        public virtual Manager Manager { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
