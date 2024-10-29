using System;
using System.Collections.Generic;

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
        public string ServiceName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<ServiceImage> ServiceImages { get; set; }
        public virtual ICollection<TypeRoomService> TypeRoomServices { get; set; }
    }
}
