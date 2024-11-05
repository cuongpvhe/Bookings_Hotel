using Bookings_Hotel.Models;

namespace Bookings_Hotel.DTO
{
    public class ServiceDTO
    {
        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public List<String>? ServiceImageUrls { get; set; }
    }
}
