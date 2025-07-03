using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookstoreBackend.Identity;
using System.Threading.Tasks;
using BookstoreBackend.DTOs.Profile;
using System.Security.Claims;
using BookstoreBackend.Data;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.DTOs.Order;
using BookstoreBackend.DTOs.Wishlist;
using BookstoreBackend.DTOs.Address;

namespace BookstoreBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookstoreContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, BookstoreContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderStatus)
                .OrderByDescending(o => o.Date)
                .Select(o => new OrderSummaryDTO
                {
                    Id = o.Id,
                    OrderDate = o.Date,
                    TotalAmount = o.TotalAmount,
                    Status = o.OrderStatus.Name
                })
                .ToListAsync();

            var wishlist = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Book.BookAuthors).ThenInclude(ba => ba.Author)
                .OrderByDescending(w => w.AddedAt)
                .Select(w => new WishlistItemDTO
                {
                    BookId = w.BookId,
                    Title = w.Book.Title,
                    AuthorName = w.Book.BookAuthors.Select(ba => ba.Author.Name).FirstOrDefault(),
                    Price = w.Book.Price,
                    ImageUrl = w.Book.Image,
                    AddedAt = w.AddedAt
                })
                .ToListAsync();

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .Select(a => new AddressDTO
                {
                    Id = a.Id,
                    Country = a.Country,
                    Region = a.Region,
                    City = a.City,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    PostalCode = a.PostalCode
                })
                .ToListAsync();

            var userProfileDto = new UserProfileDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Orders = orders,
                Wishlist = wishlist,
                Addresses = addresses
            };

            return Ok(userProfileDto);
        }

        // You can add other methods like UpdateProfile, ChangePassword etc. here
    }
}