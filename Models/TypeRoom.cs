using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class TypeRoom
    {
        public TypeRoom()
        {
            Rooms = new HashSet<Room>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
