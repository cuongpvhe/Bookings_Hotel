﻿using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Order
    {
        public Order()
        {
            Reviews = new HashSet<Review>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal? Discount { get; set; }
        public string? OrderStatus { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
