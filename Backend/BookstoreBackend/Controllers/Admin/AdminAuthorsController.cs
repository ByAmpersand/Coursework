using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookstoreBackend.Data;
using BookstoreBackend.Entities;
using BookstoreBackend.DTOs.Author;
using BookstoreBackend.DTOs.Admin;

namespace BookstoreBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/authors")]
    [ApiController]
    public class AdminAuthorsController : ControllerBase
    {
        private readonly BookstoreContext _context;
        public AdminAuthorsController(BookstoreContext context) { _context = context; }

        [HttpPost]
        public async Task<ActionResult<Author>> CreateAuthor([FromBody] CreateAuthorDTO dto)
        {
            var author = new Author
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth,
                Biography = dto.Biography
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AuthorsController.GetAuthor), "Authors", new { id = author.Id }, author);
        }

        // Тут можна додати методи PUT для оновлення та DELETE для видалення
    }
}