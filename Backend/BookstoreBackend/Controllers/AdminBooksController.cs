using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.Data;
using BookstoreBackend.Entities;
using BookstoreBackend.DTOs.Book;

namespace BookstoreBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/Admin/books")]
    [ApiController]
    public class AdminBooksController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public AdminBooksController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] CreateOrUpdateBookDTO dto)
        {
            var newBook = new Book
            {
                Title = dto.Title,
                PublisherId = dto.PublisherId,
                Isbn = dto.Isbn,
                Price = dto.Price,
                LanguageId = dto.LanguageId,
                NumberOfPages = dto.NumberOfPages,
                PublicationYear = dto.PublicationYear,
                Description = dto.Description,
                Image = dto.Image,
                StockQuantity = dto.StockQuantity,
                DateAdded = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            newBook.Genres = genres;

            newBook.BookAuthors = dto.AuthorIds.Select(authorId => new BookAuthor
            {
                AuthorId = authorId,
                OrdinalNumber = 0
            }).ToList();

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BooksController.GetBook), "Books", new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateOrUpdateBookDTO dto)
        {
            var bookToUpdate = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bookToUpdate == null) return NotFound();

            bookToUpdate.Title = dto.Title;
            bookToUpdate.PublisherId = dto.PublisherId;
            bookToUpdate.Isbn = dto.Isbn;
            bookToUpdate.Price = dto.Price;
            bookToUpdate.LanguageId = dto.LanguageId;
            bookToUpdate.NumberOfPages = dto.NumberOfPages;
            bookToUpdate.PublicationYear = dto.PublicationYear;
            bookToUpdate.Description = dto.Description;
            bookToUpdate.Image = dto.Image;
            bookToUpdate.StockQuantity = dto.StockQuantity;

            var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            bookToUpdate.Genres = genres;

            bookToUpdate.BookAuthors.Clear();
            foreach (var authorId in dto.AuthorIds)
            {
                bookToUpdate.BookAuthors.Add(new BookAuthor
                {
                    AuthorId = authorId,
                    OrdinalNumber = 0
                });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}