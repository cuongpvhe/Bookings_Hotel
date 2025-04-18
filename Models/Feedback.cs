﻿using System;
using System.Collections.Generic;

namespace Bookings_Hotel.Models
{
    public partial class Feedback
    {
        public Feedback()
        {
            FeedbackImages = new HashSet<FeedbackImage>();
        }

        public int ReviewId { get; set; }
        public int? RoomId { get; set; }
        public int? OrderId { get; set; }
        public int? AccountId { get; set; }
        public decimal? Rating { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? Comment { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Room? Room { get; set; }
        public virtual ICollection<FeedbackImage> FeedbackImages { get; set; }
    }
}
