namespace Bookings_Hotel.DTO
{
    public class TypeRoomDTO
    {
        public decimal? Price { get; set; }
        public int? TypeId { get; set; }
        public string? TypeName { get; set; } = null!;
        public int? RoomNumber { get; set; }
        public int? NumberOfChild { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfBed { get; set; }
        public string? RoomStatus { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? PriceString { get; set; }
        public string? PriceVATString { get; set; }
        public List<String> LstService {  get; set; }
    }
}
