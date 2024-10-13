using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Service
    {
        public Service()
        {
            RoomServices = new HashSet<RoomService>();
            ServiceImages = new HashSet<ServiceImage>();
        }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<RoomService> RoomServices { get; set; }
        public virtual ICollection<ServiceImage> ServiceImages { get; set; }
    }
}
