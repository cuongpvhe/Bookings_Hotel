namespace Bookings_Hotel.DTO
{
    public class RoomDTO
    {
        public int? RoomId { get; set; }
        public decimal? Price { get; set; }
        public int? TypeId { get; set; }
        public int RoomNumber { get; set; }
        public int? NumberOfChild { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfBed { get; set; }
        public string? RoomStatus { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? PriceString { get; set; }

        public RoomDTO() { }
        public RoomDTO(int roomId, int roomNumber, int? numberOfChild, int? numberOfAdult, int? numberOfBed, decimal price, string priceString)
        {
            RoomId = roomId;
            RoomNumber = roomNumber;
            NumberOfChild = numberOfChild;
            NumberOfAdult = numberOfAdult;
            NumberOfBed = numberOfBed;
            Price = price;
            PriceString = priceString;
        }
    }
}
