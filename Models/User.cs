using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class User
    {
        public User()
        {
            Bookings = new HashSet<Booking>();
            Reviews = new HashSet<Review>();
        }

        public int UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
