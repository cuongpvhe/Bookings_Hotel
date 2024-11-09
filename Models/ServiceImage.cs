using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class ServiceImage
    {
        public int ServiceImageId { get; set; }
        public string? ImageUrl { get; set; }
        public int? ServiceId { get; set; }
        public int ImageIndex { get; set; }

        public virtual Service? Service { get; set; }
    }
}
