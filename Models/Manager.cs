using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Manager
    {
        public Manager()
        {
            Bookings = new HashSet<Booking>();
            Reviews = new HashSet<Review>();
        }

        public int ManagerId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public int? Role { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
