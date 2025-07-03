namespace BookstoreBackend.DTOs.Admin
{
    public class CreateAuthorDTO
    {
        public required string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Biography { get; set; }
    }
}
