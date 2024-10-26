using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class TypeRoomImage
    {
        public int TypeRoomImageId { get; set; }
        public string? ImageUrl { get; set; }
        public int? TypeId { get; set; }
        public int? ImageIndex { get; set; }

        public virtual TypeRoom? Type { get; set; }
    }
}
