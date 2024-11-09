using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookings_Hotel.Models
{
    public partial class Service
    {
        public Service()
        {
            ServiceImages = new HashSet<ServiceImage>();
            TypeRoomServices = new HashSet<TypeRoomService>();
        }

        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Hãy nhập tên dịch vụ")]
        [MaxLength(50, ErrorMessage = "Tên dịch vụ tối đa 50 ký tự.")]
        [MinLength(2, ErrorMessage = "Tên dịch vụ tối thiểu 2 ký tự.")]
        public string ServiceName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Required(ErrorMessage = "Hãy nhập giá dịch vụ")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        [DataType(DataType.Currency, ErrorMessage = "Vui lòng nhập giá hợp lệ.")]
        public decimal? Price { get; set; }

        public string? Description { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<ServiceImage> ServiceImages { get; set; }
        public virtual ICollection<TypeRoomService> TypeRoomServices { get; set; }
    }
}
