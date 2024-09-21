using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Booking
    {
        public Booking()
        {
            Payments = new HashSet<Payment>();
        }

        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int ManagerId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingStatus { get; set; }

        public virtual Manager Manager { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
