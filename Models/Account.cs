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
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(50, ErrorMessage = "Họ và tên tối đa 50 ký tự.")]
        public string FullName { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$", ErrorMessage = "Email phải có định dạng hợp lệ @gmail.com,@yourdomain.com.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Tên người dùng không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên người dùng không có khoảng trắng hoặc ký tự đặc biệt.")]
        public string UseName { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d\W]+$", ErrorMessage = "Mật khẩu phải có ít nhất một chữ cái và một số.")]
        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        [RegularExpression(@"^\d{10,}$", ErrorMessage = "Số điện thoại phải có ít nhất 10 chữ số và chỉ chứa số.")]
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
