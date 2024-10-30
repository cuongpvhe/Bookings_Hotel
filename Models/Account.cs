using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookings_Hotel.Models
{
    public partial class Account
    {
        public Account()
        {
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
        }

        public int AccountId { get; set; }
        public int? RoleId { get; set; }
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(50, ErrorMessage = "Full Name must be at most 50 characters long.")]

        public string FullName { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$", ErrorMessage = "Email must be in a valid format, e.g., yourname@gmail.com or yourname@yourdomain.com.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must only contain letters and numbers without spaces or special characters.")]
        public string UseName { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must contain at least one letter and one number.")]
        public string Password { get; set; } = null!;
        public string? Avatar { get; set; }

        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Phone number must be at least 10 digits long and contain only numbers.")]
        public string? Phonenumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}