using BookstoreBackend.DTOs.Book;

namespace BookstoreBackend.DTOs.Publisher
{
    public class PublisherDetailsDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Country { get; set; }
        public List<BookSummaryDTO> Books { get; set; } = new List<BookSummaryDTO>();
    }
}
