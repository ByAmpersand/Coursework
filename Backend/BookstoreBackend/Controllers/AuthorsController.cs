using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookstoreBackend.DTOs.Author;
using BookstoreBackend.DTOs.Book;

namespace BookstoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public AuthorsController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            var authors = await _context.Authors
                .Select(a => new AuthorDTO { Id = a.Id, Name = a.Name })
                .OrderBy(a => a.Name)
                .ToListAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDetailsDTO>> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.BookAuthors)
                    .ThenInclude(ba => ba.Book)
                        .ThenInclude(b => b.Ratings)
                .Include(a => a.BookAuthors)
                    .ThenInclude(ba => ba.Book)
                        .ThenInclude(b => b.Genres)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return NotFound();

            var authorDetails = new AuthorDetailsDTO
            {
                Id = author.Id,
                Name = author.Name,
                DateOfBirth = author.DateOfBirth,
                Biography = author.Biography,
                Books = author.BookAuthors.Select(ba => new BookSummaryDTO
                {
                    Id = ba.Book.Id,
                    Title = ba.Book.Title,
                    Image = ba.Book.Image,
                    Price = ba.Book.Price,
                    Authors = new List<string> { author.Name },
                    Genres = ba.Book.Genres.Select(g => g.GenreName).ToList(),
                    AverageRating = ba.Book.Ratings.Any()
                ? (double?)ba.Book.Ratings.Average(r => r.RatingStars)
                : null
                }).ToList()
            };

            return Ok(authorDetails);
        }
    }
}