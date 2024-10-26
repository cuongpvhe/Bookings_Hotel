using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookings_Hotel.Models
{
    public partial class Room
    {
        public Room()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Reviews = new HashSet<Review>();
            RoomImages = new HashSet<RoomImage>();
            RoomServices = new HashSet<RoomService>();
        }

        public int RoomId { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Room type is required.")]
        public int? TypeId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Room number must be a positive integer.")]
        public int RoomNumber { get; set; }

        [Range(0, 10, ErrorMessage = "Number of children must be between 0 and 10.")]
        public int? NumberOfChild { get; set; }

        [Required(ErrorMessage = "Number of adults is required.")]
        [Range(1, 10, ErrorMessage = "Number of adults must be between 1 and 10.")]
        public int? NumberOfAdult { get; set; }

        [Required(ErrorMessage = "Number of beds is required.")]
        [Range(1, 5, ErrorMessage = "Number of beds must be between 1 and 5.")]
        public int? NumberOfBed { get; set; }

        public string? RoomStatus { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual TypeRoom? Type { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<RoomImage> RoomImages { get; set; }
        public virtual ICollection<RoomService> RoomServices { get; set; }
    }
}
