using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookstoreBackend.DTOs.Genre;
using BookstoreBackend.DTOs.Language;

namespace BookstoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public LookupController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet("genres")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GenreDTO>>> GetGenres()
        {
            var genres = await _context.Genres
                .Select(g => new GenreDTO
                {
                    Id = g.Id,
                    Name = g.GenreName
                })
                .OrderBy(g => g.Name)
                .ToListAsync();

            return Ok(genres);
        }


        [HttpGet("languages")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LanguageDTO>>> GetLanguages()
        {
            var languages = await _context.Languages
                .Select(l => new LanguageDTO
                {
                    Id = l.Id,
                    Name = l.LanguageName
                })
                .OrderBy(l => l.Name)
                .ToListAsync();

            return Ok(languages);
        }
    }
}