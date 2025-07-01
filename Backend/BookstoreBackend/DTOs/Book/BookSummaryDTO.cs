namespace BookstoreBackend.DTOs.Book
{
    public class BookSummaryDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required List<string> Authors { get; set; }
        public required List<string> Genres { get; set; }
        public decimal Price { get; set; }
        public required string Image { get; set; }
        public double? AverageRating { get; set; }
    }
}