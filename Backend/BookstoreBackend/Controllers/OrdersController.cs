using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookstoreBackend.Data;
using BookstoreBackend.DTOs.Order;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.Entities;

namespace BookstoreBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BookstoreContext _context;

        public OrdersController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderBooks)
                    .ThenInclude(ob => ob.Book)
                .OrderByDescending(o => o.Date)
                .Select(o => new OrderDetailsDTO
                {
                    Id = o.Id,
                    OrderDate = o.Date,
                    TotalAmount = o.TotalAmount,
                    Status = o.OrderStatus.Name,
                    RecipientName = o.RecipientName,
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (createOrderDto.OrderItems == null || !createOrderDto.OrderItems.Any())
            {
                return BadRequest("Order must contain at least one item.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    Date = DateTime.UtcNow,
                    StatusId = 1,
                    TotalAmount = 0,
                    ShippingCountry = createOrderDto.ShippingCountry,
                    ShippingRegion = createOrderDto.ShippingRegion,
                    ShippingCity = createOrderDto.ShippingCity,
                    ShippingAddressLine1 = createOrderDto.ShippingAddressLine1,
                    ShippingAddressLine2 = createOrderDto.ShippingAddressLine2,
                    ShippingPostalCode = createOrderDto.ShippingPostalCode,
                    RecipientName = createOrderDto.RecipientName,
                    RecipientPhoneNumber = createOrderDto.RecipientPhoneNumber
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in createOrderDto.OrderItems)
                {
                    var book = await _context.Books.FindAsync(item.BookId);
                    if (book == null) throw new Exception($"Book with ID {item.BookId} not found.");
                    if (book.StockQuantity < item.Quantity) throw new Exception($"Not enough stock for book '{book.Title}'.");

                    var orderBook = new OrderBook
                    {
                        OrderId = order.Id,
                        BookId = book.Id,
                        BookQuantity = item.Quantity,
                        BookPrice = book.Price
                    };
                    _context.OrderBooks.Add(orderBook);

                    var stockLog = new BookStockLog
                    {
                        BookId = book.Id,
                        UserId = userId,
                        ChangeTypeId = 2,
                        Quantity = item.Quantity,
                        ChangeDate = DateTime.UtcNow
                    };
                    _context.BookStockLogs.Add(stockLog);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "Order created successfully.", orderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = $"Failed to create order. {ex.Message}" });
            }
        }
    }
}