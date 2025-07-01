using System.ComponentModel.DataAnnotations;

namespace BookstoreBackend.DTOs.Ratings
{
    public class SubmitRatingDTO
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public decimal Stars { get; set; }

        public string? Review { get; set; }
    }
}