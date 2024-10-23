namespace Bookings_Hotel.DTO
{
    public class PaymentDTO
    {
        public int? OrderID { get; set; }
        public decimal? Money { get; set; }
        public string? MoneyString { get; set; }
        public string? CurrencyCode { get; set; }
        public readonly string BankName = "VietinBank";
        public readonly string BankCode = "970415";
        public readonly string AccountNumber = "103873672562";
        public readonly string AccountName = "Chu Hai Dang";

        public PaymentDTO()
        {
        }

        public PaymentDTO(int? orderID, decimal? money, string? currencyCode,string? moneyString)
        {
            OrderID = orderID;
            Money = money;
            CurrencyCode = currencyCode;
            MoneyString = moneyString;
        }
    }
}
