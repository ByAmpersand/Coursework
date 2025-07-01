using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BookstoreBackend.DTOs.Ratings;
using BookstoreBackend.Entities;

namespace BookstoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public RatingsController(BookstoreContext context)
        {
            _context = context;
        }


        [HttpGet("Books/{bookId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetRatingsForBook(int bookId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .Select(r => new RatingDTO
                {
                    UserName = r.User.FirstName,
                    Stars = r.RatingStars,
                    Review = r.Review
                })
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitRating([FromBody] SubmitRatingDTO submitRatingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var bookExists = await _context.Books.AnyAsync(b => b.Id == submitRatingDto.BookId);
            if (!bookExists)
            {
                return NotFound(new { message = "Book not found." });
            }

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == submitRatingDto.BookId);

            if (existingRating != null)
            {
                existingRating.RatingStars = submitRatingDto.Stars;
                existingRating.Review = submitRatingDto.Review;
            }
            else
            {
                var newRating = new Rating
                {
                    UserId = userId,
                    BookId = submitRatingDto.BookId,
                    RatingStars = submitRatingDto.Stars,
                    Review = submitRatingDto.Review
                };
                _context.Ratings.Add(newRating);
            }

            await _context.SaveChangesAsync();

            var newAverageRating = await _context.Ratings
                .Where(r => r.BookId == submitRatingDto.BookId)
                .AverageAsync(r => r.RatingStars);

            return Ok(new { message = "Rating submitted successfully.", averageRating = newAverageRating });
        }
    }
}