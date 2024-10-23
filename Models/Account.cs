using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        public string UseName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
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
