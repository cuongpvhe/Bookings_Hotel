using System;
using System.Collections.Generic;

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
        public decimal Price { get; set; }
        public int? TypeId { get; set; }
        public int RoomNumber { get; set; }
        public int? NumberOfChild { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfBed { get; set; }
        public string? RoomStatus { get; set; }
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
