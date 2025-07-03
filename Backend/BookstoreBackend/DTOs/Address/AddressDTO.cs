namespace BookstoreBackend.DTOs.Address
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public required string Country { get; set; }
        public required string Region { get; set; }
        public required string City { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string PostalCode { get; set; }
    }
}
