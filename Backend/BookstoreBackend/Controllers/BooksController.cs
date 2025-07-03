using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.Data;
using BookstoreBackend.DTOs.Book;
using BookstoreBackend.Services;
using BookstoreBackend.DTOs.GoogleBooks;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookstoreContext _context;
        private readonly IGoogleBooksService _googleBooksService;

        public BooksController(BookstoreContext context, IGoogleBooksService googleBooksService)
        {
            _context = context;
            _googleBooksService = googleBooksService;
        }

        // Search via Google Books API (for admin panel)
        [HttpGet("google-search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GoogleBookItem>>> SearchGoogleBooks([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }
            var results = await _googleBooksService.SearchBooksAsync(query);
            return Ok(results);
        }

        // THE ONLY ENDPOINT FOR GETTING/SEARCHING/FILTERING BOOKS
        [HttpGet]
        public async Task<ActionResult> GetBooks(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? minYear,
            [FromQuery] int? maxYear,
            [FromQuery] int? minPages,
            [FromQuery] int? maxPages,
            [FromQuery] List<int>? genreIds,
            [FromQuery] List<int>? languageIds,
            [FromQuery] List<int>? authorIds,
            [FromQuery] List<int>? publisherIds,
            [FromQuery] bool? inStock,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12
        )
        {
            var queryable = _context.Books.AsQueryable();

            // --- SEARCH LOGIC ---
            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.Trim().ToLower();
                queryable = queryable.Where(b =>
                    b.Title.ToLower().Contains(lowerQuery) ||
                    b.Isbn.Contains(lowerQuery) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.ToLower().Contains(lowerQuery))
                );
            }
            if (minPrice.HasValue) queryable = queryable.Where(b => b.Price >= minPrice.Value);

            if (maxPrice.HasValue) queryable = queryable.Where(b => b.Price <= maxPrice.Value);
            if (minYear.HasValue) queryable = queryable.Where(b => b.PublicationYear >= minYear.Value);
            if (maxYear.HasValue) queryable = queryable.Where(b => b.PublicationYear <= maxYear.Value);
            if (minPages.HasValue) queryable = queryable.Where(b => b.NumberOfPages >= minPages.Value);
            if (maxPages.HasValue) queryable = queryable.Where(b => b.NumberOfPages <= maxPages.Value);
            if (genreIds != null && genreIds.Any()) queryable = queryable.Where(b => b.Genres.Any(g => genreIds.Contains(g.Id)));
            if (languageIds != null && languageIds.Any()) queryable = queryable.Where(b => languageIds.Contains(b.LanguageId));
            if (authorIds != null && authorIds.Any()) queryable = queryable.Where(b => b.BookAuthors.Any(ba => authorIds.Contains(ba.AuthorId)));
            if (publisherIds != null && publisherIds.Any()) queryable = queryable.Where(b => publisherIds.Contains(b.PublisherId));
            if (inStock.HasValue && inStock.Value) queryable = queryable.Where(b => b.StockQuantity > 0);

            // --- PAGINATION: Get total count BEFORE skipping/taking ---
            var totalCount = await queryable.CountAsync();

            // --- SORTING LOGIC ---
            if (!string.IsNullOrEmpty(sortBy) && sortBy.Equals("rating_desc", StringComparison.OrdinalIgnoreCase))
            {
                queryable = queryable.OrderByDescending(b => b.Ratings.Any() ? b.Ratings.Average(r => r.RatingStars) : 0);
            }
            else
            {
                queryable = queryable.OrderByDescending(b => b.DateAdded);
            }

            // --- PAGINATION: Apply skip and take ---
            var books = await queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookSummaryDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                    Genres = b.Genres.Select(g => g.GenreName).ToList(),
                    Price = b.Price,
                    Image = b.Image,
                    AverageRating = b.Ratings.Any() ? (double?)b.Ratings.Average(r => r.RatingStars) : null,
                    StockQuantity = b.StockQuantity
                })
                .ToListAsync();

            // Return the data and pagination info in a structured object
            return Ok(new
            {
                Books = books,
                TotalCount = totalCount
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var bookDto = await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                    PublisherName = b.Publisher.Name,
                    PublicationYear = b.PublicationYear,
                    Isbn = b.Isbn,
                    NumberOfPages = b.NumberOfPages,
                    LanguageName = b.Language.LanguageName,
                    Genres = b.Genres.Select(g => g.GenreName).ToList(),
                    Price = b.Price,
                    Description = b.Description,
                    Image = b.Image,
                    DateAdded = b.DateAdded,
                    StockQuantity = b.StockQuantity,
                    AverageRating = b.Ratings.Any() ? (double?)b.Ratings.Average(r => r.RatingStars) : null,
                    RatingsCount = b.Ratings.Count()
                })
                .FirstOrDefaultAsync();

            if (bookDto == null)
            {
                return NotFound();
            }

            return Ok(bookDto);
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using BookstoreBackend.Data;
//using BookstoreBackend.Entities;
//using Microsoft.AspNetCore.Authorization;
//using BookstoreBackend.DTOs.Book;
//using BookstoreBackend.DTOs.GoogleBooks;
//using BookstoreBackend.Services;

//namespace BookstoreBackend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BooksController : ControllerBase
//    {
//        private readonly BookstoreContext _context;
//        private readonly IGoogleBooksService _googleBooksService;

//        public BooksController(BookstoreContext context, IGoogleBooksService googleBooksService)
//        {
//            _context = context;
//            _googleBooksService = googleBooksService;
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpGet("google-search")]
//        public async Task<ActionResult<IEnumerable<GoogleBookItem>>> SearchGoogleBooks([FromQuery] string query)
//        {
//            if (string.IsNullOrWhiteSpace(query))
//            {
//                return BadRequest("Search query cannot be empty.");
//            }
//            var results = await _googleBooksService.SearchBooksAsync(query);
//            return Ok(results);
//        }

//        [HttpGet("search")]
//        public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks([FromQuery] string query)
//        {
//            if (string.IsNullOrWhiteSpace(query))
//                return BadRequest("Search query cannot be empty.");

//            query = query.Trim().ToLower();

//            var books = await _context.Books
//                .Include(b => b.BookAuthors)
//                    .ThenInclude(ba => ba.Author)
//                .Include(b => b.Genres)
//                .Include(b => b.Publisher)
//                .Include(b => b.Language)
//                .Where(b =>
//                    b.Title.ToLower().Contains(query) ||
//                    b.Isbn.ToLower().Contains(query) ||
//                    b.Publisher.Name.ToLower().Contains(query) ||
//                    b.BookAuthors.Any(ba => ba.Author.Name.ToLower().Contains(query)) ||
//                    b.Genres.Any(g => g.GenreName.ToLower().Contains(query))
//                )
//                .Select(b => new BookSummaryDTO
//                {
//                    Id = b.Id,
//                    Title = b.Title,
//                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
//                    Genres = b.Genres.Select(g => g.GenreName).ToList(),
//                    Price = b.Price,
//                    Image = b.Image,
//                    AverageRating = b.Ratings.Any() ? (double?)b.Ratings.Average(r => r.RatingStars) : null,
//                    StockQuantity = b.StockQuantity
//                })
//            .ToListAsync();

//            return Ok(books);
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<BookSummaryDTO>>> GetBooks(
//            [FromQuery] string? sortBy,
//            [FromQuery] decimal? minPrice,
//            [FromQuery] decimal? maxPrice,
//            [FromQuery] int? minYear,
//            [FromQuery] int? maxYear,
//            [FromQuery] int? minPages,
//            [FromQuery] int? maxPages,
//            [FromQuery] List<int>? genreIds,
//            [FromQuery] List<int>? languageIds,
//            [FromQuery] List<int>? authorIds,
//            [FromQuery] List<int>? publisherIds,
//            [FromQuery] bool? inStock)
//        {
//            var query = _context.Books.AsQueryable();

//            if (minPrice.HasValue)
//            {
//                query = query.Where(b => b.Price >= minPrice.Value);
//            }
//            if (maxPrice.HasValue)
//            {
//                query = query.Where(b => b.Price <= maxPrice.Value);
//            }

//            if (minYear.HasValue)
//            {
//                query = query.Where(b => b.PublicationYear >= minYear.Value);
//            }
//            if (maxYear.HasValue)
//            {
//                query = query.Where(b => b.PublicationYear <= maxYear.Value);
//            }

//            if (minPages.HasValue)
//            {
//                query = query.Where(b => b.NumberOfPages >= minPages.Value);
//            }
//            if (maxPages.HasValue)
//            {
//                query = query.Where(b => b.NumberOfPages <= maxPages.Value);
//            }

//            if (genreIds != null && genreIds.Any())
//            {
//                query = query.Where(b => b.Genres.Any(g => genreIds.Contains(g.Id)));
//            }

//            if (languageIds != null && languageIds.Any())
//            {
//                query = query.Where(b => languageIds.Contains(b.LanguageId));
//            }

//            if (authorIds != null && authorIds.Any())
//            {
//                query = query.Where(b => b.BookAuthors.Any(ba => authorIds.Contains(ba.AuthorId)));
//            }

//            if (publisherIds != null && publisherIds.Any())
//            {
//                query = query.Where(b => publisherIds.Contains(b.PublisherId));
//            }

//            if (inStock.HasValue && inStock.Value)
//            {
//                query = query.Where(b => b.StockQuantity > 0);
//            }

//            if (!string.IsNullOrEmpty(sortBy) && sortBy.Equals("rating_desc", StringComparison.OrdinalIgnoreCase))
//            {
//                query = query.OrderByDescending(b => b.Ratings.Any() ? b.Ratings.Average(r => r.RatingStars) : 0);
//            }
//            else
//            {
//                query = query.OrderByDescending(b => b.DateAdded);
//            }

//            var books = await query
//            .Select(b => new BookSummaryDTO
//            {
//                Id = b.Id,
//                Title = b.Title,
//                Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
//                Genres = b.Genres.Select(g => g.GenreName).ToList(),
//                Price = b.Price,
//                Image = b.Image,
//                AverageRating = b.Ratings.Any() ? (double?)b.Ratings.Average(r => r.RatingStars): null,
//                StockQuantity = b.StockQuantity
//            })
//            .ToListAsync();

//            return Ok(books);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<BookDTO>> GetBook(int id)
//        {
//            var bookDto = await _context.Books
//                .Where(b => b.Id == id)
//                .Include(b => b.BookAuthors)
//                    .ThenInclude(ba => ba.Author)
//                .Include(b => b.Genres)
//                .Include(b => b.Publisher)
//                .Include(b => b.Language)
//                .Select(b => new BookDTO
//                {
//                    Id = b.Id,
//                    Title = b.Title,
//                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
//                    PublisherName = b.Publisher != null ? b.Publisher.Name : null,
//                    PublicationYear = b.PublicationYear,
//                    Isbn = b.Isbn,
//                    NumberOfPages = b.NumberOfPages,
//                    LanguageName = b.Language != null ? b.Language.LanguageName : null,
//                    Genres = b.Genres.Select(g => g.GenreName).ToList(),
//                    Price = b.Price,
//                    Description = b.Description,
//                    Image = b.Image,
//                    DateAdded = b.DateAdded,
//                    StockQuantity = b.StockQuantity
//                })
//                .FirstOrDefaultAsync();

//            if (bookDto == null)
//            {
//                return NotFound();
//            }

//            return Ok(bookDto);
//        }

//        private bool BookExists(int id)
//        {
//            return _context.Books.Any(e => e.Id == id);
//        }
//    }
//}