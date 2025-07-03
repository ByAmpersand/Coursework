using BookstoreBackend.DTOs.Address;
using BookstoreBackend.DTOs.Order;
using BookstoreBackend.DTOs.Wishlist;

namespace BookstoreBackend.DTOs.Profile
{
    public class UserProfileDTO
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public List<OrderSummaryDTO> Orders { get; set; } = new();
        public List<WishlistItemDTO> Wishlist { get; set; } = new();
        public List<AddressDTO> Addresses { get; set; } = new();
    }
}
