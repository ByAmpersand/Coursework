using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookstoreBackend.Data;
using BookstoreBackend.Entities;
using BookstoreBackend.DTOs.Publisher;
using BookstoreBackend.DTOs.Admin;

namespace BookstoreBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/publishers")]
    [ApiController]
    public class AdminPublishersController : ControllerBase
    {
        private readonly BookstoreContext _context;
        public AdminPublishersController(BookstoreContext context) { _context = context; }

        [HttpPost]
        public async Task<ActionResult<Publisher>> CreatePublisher([FromBody] CreatePublisherDTO dto)
        {
            var publisher = new Publisher
            {
                Name = dto.Name,
                Description = dto.Description,
                Country = dto.Country
            };

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PublishersController.GetPublisher), "Publishers", new { id = publisher.Id }, publisher);
        }

        // Тут можна додати методи PUT та DELETE
    }
}