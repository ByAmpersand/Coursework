using System.ComponentModel.DataAnnotations;

namespace BookstoreBackend.DTOs.Wishlist
{
    public class AddToWishlistDTO
    {
        [Required]
        public int BookId { get; set; }
    }
}