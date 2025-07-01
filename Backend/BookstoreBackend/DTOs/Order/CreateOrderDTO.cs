using System.ComponentModel.DataAnnotations;

namespace BookstoreBackend.DTOs.Order
{
    // Описує один товар у кошику
    public class OrderItemDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }

    // Основний DTO для створення замовлення
    public class CreateOrderDTO
    {
        [Required]
        public required List<OrderItemDTO> OrderItems { get; set; }

        // Поля для адреси доставки (копіюємо, як і домовлялися)
        [Required]
        public required string ShippingCountry { get; set; }
        [Required]
        public required string ShippingRegion { get; set; }
        [Required]
        public required string ShippingCity { get; set; }
        [Required]
        public required string ShippingAddressLine1 { get; set; }
        public string? ShippingAddressLine2 { get; set; }
        [Required]
        public required string ShippingPostalCode { get; set; }
        [Required]
        public required string RecipientName { get; set; }
        [Required]
        public required string RecipientPhoneNumber { get; set; }
    }
}