using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class FeedbackImage
    {
        public int ReviewImageId { get; set; }
        public string? ImageUrl { get; set; }
        public int? ReviewId { get; set; }

        public virtual Feedback? Review { get; set; }
    }
}
