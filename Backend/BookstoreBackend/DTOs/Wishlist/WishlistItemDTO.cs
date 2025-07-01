namespace BookstoreBackend.DTOs.Wishlist
{
    public class WishlistItemDTO
    {
        public int BookId { get; set; }
        public required string Title { get; set; }
        public string? AuthorName { get; set; } // Може бути кілька авторів, поки для простоти зробимо одного
        public decimal Price { get; set; }
        public required string ImageUrl { get; set; }
        public DateTime AddedAt { get; set; } // Коли книга була додана
    }
}
