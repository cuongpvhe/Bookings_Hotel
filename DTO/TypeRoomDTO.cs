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
        public int? MaximumExtraAdult { get; set; }
        public int? MaximumExtraChild { get; set; }
        public int? NumberOfBed { get; set; }
        public string? RoomStatus { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? PriceString { get; set; }
        public string? PriceVATString { get; set; }
        public decimal? ExtraAdultFee { get; set; }
        public decimal? ExtraChildFee { get; set; }
        public string? ExtraAdultFeeString {  get; set; }
        public string? ExtraChildFeeString { get; set; }
        public List<String>? LstService { get; set; }
        public List<ServiceDTO>? LstServiceObject {  get; set; }
        public List<String>? LstImage { get; set; }
    }
}
