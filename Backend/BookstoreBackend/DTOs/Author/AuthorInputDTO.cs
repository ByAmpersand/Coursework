namespace BookstoreBackend.DTOs.Author
{
    public class AuthorInputDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Biography { get; set; }
    }
}
