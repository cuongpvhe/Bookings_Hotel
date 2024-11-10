using Bookings_Hotel.Models;

namespace Bookings_Hotel.DTO
{
    public class OrderDTO
    {
        public int? OrderId { get; set; }
        public decimal TotalMoney { get; set; }
        public string? Note { get; set; }
        public string? RoomType { get; set; }
        public string? RoomNumber { get; set; }
        public string? checkinDate { get; set; }
        public string? checkoutDate { get;set; }
    }
}
