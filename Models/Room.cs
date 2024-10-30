using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Room
    {
        public Room()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int RoomId { get; set; }
        public int? TypeId { get; set; }
        public int RoomNumber { get; set; }
        public string? RoomStatus { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? Floor { get; set; }

        public virtual TypeRoom? Type { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
