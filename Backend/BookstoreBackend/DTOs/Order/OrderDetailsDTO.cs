namespace BookstoreBackend.DTOs.Order
{
    public class OrderBookDTO
    {
        public int BookId { get; set; }
        public required string Title { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTimeOfPurchase { get; set; }
    }

    public class OrderDetailsDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }
        public required string ShippingAddress { get; set; }
        public required string RecipientName { get; set; }
        public required List<OrderBookDTO> Items { get; set; }
    }
}