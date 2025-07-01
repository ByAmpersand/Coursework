namespace BookstoreBackend.DTOs.Ratings
{
    public class RatingDTO
    {
        public required string UserName { get; set; }
        public decimal Stars { get; set; }
        public string? Review { get; set; }
    }
}
