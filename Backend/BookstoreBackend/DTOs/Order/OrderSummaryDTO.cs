namespace BookstoreBackend.DTOs.Order
{
    public class OrderSummaryDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }
    }
}
