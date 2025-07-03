using BookstoreBackend.DTOs.Order;

namespace BookstoreBackend.DTOs.Admin.Order
{
    public class AdminOrderDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }

        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public required string RecipientPhoneNumber { get; set; }
        public required string ShippingAddress { get; set; }

        public required List<OrderBookDTO> Items { get; set; }
    }
}