using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class TypeRoomService
    {
        public int TypeServiceId { get; set; }
        public int? TypeId { get; set; }
        public int? ServiceId { get; set; }

        public virtual Service? Service { get; set; }
        public virtual TypeRoom? Type { get; set; }
    }
}
