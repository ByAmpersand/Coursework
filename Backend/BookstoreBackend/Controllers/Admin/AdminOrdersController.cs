using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.Data;
using BookstoreBackend.DTOs.Admin.Order;
using BookstoreBackend.DTOs.Order;

namespace BookstoreBackend.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/Admin/orders")]
    [ApiController]
    public class AdminOrdersController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public AdminOrdersController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminOrderDTO>>> GetOrders(
            [FromQuery] int? orderId,
            [FromQuery] string? phone,
            [FromQuery] string? name,
            [FromQuery] string? email)
        {
            var query = _context.Orders.AsQueryable();

            if (orderId.HasValue)
            {
                query = query.Where(o => o.Id == orderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(o => o.RecipientPhoneNumber.Contains(phone));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(o => o.User.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var lowerName = name.ToLower();
                query = query.Where(o =>
                    (o.User.FirstName.ToLower() + " " + o.User.LastName.ToLower()).Contains(lowerName) ||
                    o.RecipientName.ToLower().Contains(lowerName)
                );
            }

            var orders = await query
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderBooks)
                    .ThenInclude(ob => ob.Book)
                .OrderByDescending(o => o.Date)
                .Select(o => new AdminOrderDTO
                {
                    OrderId = o.Id,
                    OrderDate = o.Date,
                    TotalAmount = o.TotalAmount,
                    Status = o.OrderStatus.Name,
                    CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                    CustomerEmail = o.User.Email,
                    RecipientPhoneNumber = o.RecipientPhoneNumber,
                    ShippingAddress = $"{o.ShippingCountry}, {o.ShippingCity}, {o.ShippingAddressLine1}",
                    Items = o.OrderBooks.Select(ob => new OrderBookDTO
                    {
                        BookId = ob.BookId,
                        Title = ob.Book.Title,
                        Quantity = ob.BookQuantity,
                        PriceAtTimeOfPurchase = ob.BookPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDTO dto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var statusExists = await _context.OrderStatuses.AnyAsync(s => s.Id == dto.StatusId);
            if (!statusExists)
            {
                return BadRequest("Invalid status ID.");
            }

            order.StatusId = dto.StatusId;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}