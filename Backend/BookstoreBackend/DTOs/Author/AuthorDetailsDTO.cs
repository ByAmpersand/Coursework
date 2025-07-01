using BookstoreBackend.DTOs.Book;

namespace BookstoreBackend.DTOs.Author
{
    public class AuthorDetailsDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Biography { get; set; }
        public List<BookSummaryDTO> Books { get; set; } = new List<BookSummaryDTO>();
    }
}
