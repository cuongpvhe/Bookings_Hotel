using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Order
    {
        public Order()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal? Discount { get; set; }
        public string? OrderStatus { get; set; }
        public int? AccountId { get; set; }
        public string? Note { get; set; }
        public string? PaymentCode { get; set; }
        public int? NumberExtraAdult { get; set; }
        public int? NumberExtraChild { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
