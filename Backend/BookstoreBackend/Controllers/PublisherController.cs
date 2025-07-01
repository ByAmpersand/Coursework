using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookstoreBackend.DTOs.Publisher;
using BookstoreBackend.DTOs.Book;

namespace BookstoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public PublishersController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublisherDTO>>> GetPublishers()
        {
            var publishers = await _context.Publishers
                .Select(p => new PublisherDTO { Id = p.Id, Name = p.Name })
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Ok(publishers);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PublisherDetailsDTO>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers
             .Include(p => p.Books)
                 .ThenInclude(b => b.BookAuthors)
                     .ThenInclude(ba => ba.Author)
             .Include(p => p.Books)
                 .ThenInclude(b => b.Genres)
             .Include(p => p.Books)
                 .ThenInclude(b => b.Ratings)
             .FirstOrDefaultAsync(p => p.Id == id);

            if (publisher == null) return NotFound();

            var publisherDetails = new PublisherDetailsDTO
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Description = publisher.Description,
                Country = publisher.Country,
                Books = publisher.Books.Select(b => new BookSummaryDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Image = b.Image,
                    Price = b.Price,
                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                    Genres = b.Genres.Select(g => g.GenreName).ToList(),
                    AverageRating = b.Ratings.Any()
                ? (double?)b.Ratings.Average(r => r.RatingStars)
                : null
                }).ToList()
            };

            return Ok(publisherDetails);
        }
    }
}