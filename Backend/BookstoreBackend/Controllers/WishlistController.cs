using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookstoreBackend.Identity;
using BookstoreBackend.Data;
using BookstoreBackend.Entities;
using BookstoreBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BookstoreBackend.DTOs.Wishlist;

namespace BookstoreBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly BookstoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(BookstoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WishlistItemDTO>>> GetWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var wishlistItems = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Book)
                    .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
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

            return Ok(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDTO addToWishlistDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bookExists = await _context.Books.AnyAsync(b => b.Id == addToWishlistDto.BookId);
            if (!bookExists)
            {
                return NotFound(new { message = "Book not found" });
            }

            var alreadyInWishlist = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.BookId == addToWishlistDto.BookId);
            if (alreadyInWishlist)
            {
                return Conflict(new { message = "Book is already in the wishlist" });
            }

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                BookId = addToWishlistDto.BookId,
                AddedAt = DateTime.UtcNow
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book added to wishlist successfully" });
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveFromWishlist(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

            if (wishlistItem == null)
            {
                return NotFound(new { message = "Item not found in wishlist" });
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book removed from wishlist successfully" });
        }
    }
}