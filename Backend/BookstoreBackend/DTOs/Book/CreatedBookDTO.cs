namespace BookstoreBackend.DTOs.Book
{
    public class CreatedBookDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required List<string> Authors { get; set; }
        public required List<string> Genres { get; set; }
    }
}
