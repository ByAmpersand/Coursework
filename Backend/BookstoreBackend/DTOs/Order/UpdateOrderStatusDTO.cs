using System.ComponentModel.DataAnnotations;

namespace BookstoreBackend.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Status ID must be a positive integer.")]
        public int StatusId { get; set; }
    }
}