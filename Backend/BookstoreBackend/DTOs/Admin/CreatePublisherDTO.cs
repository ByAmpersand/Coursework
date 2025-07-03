namespace BookstoreBackend.DTOs.Admin
{
    public class CreatePublisherDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Country { get; set; }
    }
}
