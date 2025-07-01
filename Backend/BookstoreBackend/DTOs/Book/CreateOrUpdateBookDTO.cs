using System.ComponentModel.DataAnnotations;
namespace BookstoreBackend.DTOs.Book
{
    public class CreateOrUpdateBookDTO
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public int PublisherId { get; set; }
        [Required]
        public required string Isbn { get; set; }
        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int NumberOfPages { get; set; }
        [Required]
        public int PublicationYear { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required string Image { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        [Required]
        public required List<int> AuthorIds { get; set; }
        [Required]
        public required List<int> GenreIds { get; set; }
    }
}