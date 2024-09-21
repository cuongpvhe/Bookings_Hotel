using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Room
    {
        public Room()
        {
            Bookings = new HashSet<Booking>();
            Reviews = new HashSet<Review>();
        }

        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string? RoomType { get; set; }
        public int? NumberOfAdults { get; set; }
        public int? NumberOfChildren { get; set; }
        public int? NumberOfBeds { get; set; }
        public decimal? PricePerNight { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
