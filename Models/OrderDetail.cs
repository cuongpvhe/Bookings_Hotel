using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class OrderDetail
    {
        public int OdId { get; set; }
        public int? RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int? OrderId { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Room? Room { get; set; }
    }
}
