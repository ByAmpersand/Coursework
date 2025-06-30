using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.Data;
using BookstoreBackend.Entities;
using BookstoreBackend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public BooksController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            query = query.Trim().ToLower();

            var books = await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Genres)
                .Include(b => b.Publisher)
                .Include(b => b.Language)
                .Where(b =>
                    b.Title.ToLower().Contains(query) ||
                    b.Isbn.ToLower().Contains(query) ||
                    b.Publisher.Name.ToLower().Contains(query) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.ToLower().Contains(query)) ||
                    b.Genres.Any(g => g.GenreName.ToLower().Contains(query))
                )
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Isbn = b.Isbn,
                    Genres = b.Genres.Select(g => g.GenreName).ToList(),
                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                    PublisherName = b.Publisher.Name,
                    PublicationYear = b.PublicationYear,
                    NumberOfPages = b.NumberOfPages,
                    LanguageName = b.Language.LanguageName,
                    Price = b.Price,
                    Description = b.Description,
                    Image = b.Image,
                    DateAdded = b.DateAdded
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books
                // Якщо у вас Book має ICollection<BookAuthor> BookAuthors для зв'язку з авторами:
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                // Якщо Book має ICollection<Genre> Genres (і EF Core налаштований на неявну таблицю зв'язку):
                .Include(b => b.Genres) // Просто включаємо колекцію жанрів
                                        // Якщо у вас явна проміжна таблиця BookGenre і Book має ICollection<BookGenre> BookGenres:
                                        // .Include(b => b.BookGenres)
                                        //     .ThenInclude(bg => bg.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Language)
                .Select(b => new BookDTO // Тут відбувається магія трансформації!
                {
                    Id = b.Id,
                    Title = b.Title,
                    // ... інші поля вашого BookDto ...
                    Price = b.Price,
                    PublicationYear = b.PublicationYear,
                    Isbn = b.Isbn, // Якщо додали в DTO
                    NumberOfPages = b.NumberOfPages, // Якщо додали в DTO
                    Description = b.Description, // Якщо додали в DTO
                    Image = b.Image, // Якщо додали в DTO
                    DateAdded = b.DateAdded, // Якщо додали в DTO

                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(), // З колекції BookAuthor беремо імена авторів

                    // Трансформація ICollection<Genre> в List<string>
                    Genres = b.Genres.Select(g => g.GenreName).ToList(), // З колекції Genre беремо назви жанрів (припускаючи, що Genre має властивість GenreName)

                    PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                    LanguageName = b.Language != null ? b.Language.LanguageName : null
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/5
        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var bookDto = await _context.Books
                .Where(b => b.Id == id) // Спочатку фільтруємо книгу за ID
                .Include(b => b.BookAuthors) // Включаємо зв'язки для авторів
                    .ThenInclude(ba => ba.Author) // Потім самих авторів через BookAuthor
                .Include(b => b.Genres)     // Включаємо колекцію пов'язаних жанрів напряму
                                            // (Якщо Book має ICollection<Genre> Genres і EF Core керує M2M неявно)
                .Include(b => b.Publisher)  // Включаємо видавництво
                .Include(b => b.Language)   // Включаємо мову
                .Select(b => new BookDTO // Проектуємо результат в BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Authors = b.BookAuthors.Select(ba => ba.Author.Name).ToList(), // Отримуємо список імен авторів
                    PublisherName = b.Publisher != null ? b.Publisher.Name : null, // Назва видавництва
                    PublicationYear = b.PublicationYear,
                    Isbn = b.Isbn,
                    NumberOfPages = b.NumberOfPages,
                    LanguageName = b.Language != null ? b.Language.LanguageName : null, // Назва мови
                    Genres = b.Genres.Select(g => g.GenreName).ToList(), // Отримуємо список назв жанрів
                                                                         // Припускаючи, що ваша сутність Genre має властивість GenreName
                    Price = b.Price,
                    Description = b.Description,
                    Image = b.Image,
                    DateAdded = b.DateAdded
                })
                .FirstOrDefaultAsync(); // Отримуємо один результат або null

            if (bookDto == null)
            {
                return NotFound(); // Якщо книгу з таким ID не знайдено
            }

            return Ok(bookDto); // Повертаємо DTO книги
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
