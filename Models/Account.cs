using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
        }

        public int AccountId { get; set; }
        public int? RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime? Dob { get; set; }
        public string Email { get; set; } = null!;
        public string UseName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Phonenumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
