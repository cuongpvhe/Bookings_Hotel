using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class TypeRoom
    {
        public TypeRoom()
        {
            Rooms = new HashSet<Room>();
            TypeRoomImages = new HashSet<TypeRoomImage>();
            TypeRoomServices = new HashSet<TypeRoomService>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public int? NumberOfChild { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfBed { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool? Deleted { get; set; }
        public int? MaximumExtraAdult { get; set; }
        public int? MaximumExtraChild { get; set; }
        public decimal? ExtraAdultFee { get; set; }
        public decimal? ExtraChildFee { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<TypeRoomImage> TypeRoomImages { get; set; }
        public virtual ICollection<TypeRoomService> TypeRoomServices { get; set; }
    }
}
