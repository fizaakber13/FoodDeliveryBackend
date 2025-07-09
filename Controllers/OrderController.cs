using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.DTOs;
using FoodDeliveryBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Restaurant)                          
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)                 
                .ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem) 
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }
        
        [HttpGet("user/{userId}/search")]
        public async Task<IActionResult> SearchOrdersByRestaurant(int userId, [FromQuery] string restaurantName = "")
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId &&
                       (string.IsNullOrEmpty(restaurantName) ||
                        o.Restaurant!.Name.Contains(restaurantName)))
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate) 
                .ToListAsync();

            return Ok(orders);
        }


        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder([FromBody] OrderDto orderDto)
        {
            try
            {
                var order = new Order
                {
                    UserId = orderDto.UserId,
                    RestaurantId = orderDto.RestaurantId,
                    OrderDate = orderDto.OrderDate,
                    TotalAmount = orderDto.TotalAmount,
                    Status = orderDto.Status,
                    Address = orderDto.Address,
                    PaymentMethod = orderDto.PaymentMethod
                };

                
                foreach (var itemDto in orderDto.OrderItems)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        MenuItemId = itemDto.MenuItemId,
                        Quantity = itemDto.Quantity,
                        Price = itemDto.Price,
                        Order = order 
                    });
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("❌ DB Update Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("👉 Inner Exception: " + ex.InnerException.Message);
                return StatusCode(500, "DB error: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ General Error: " + ex.Message);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }



       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] JsonElement body)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            if (body.TryGetProperty("status", out var statusElement))
            {
                existingOrder.Status = statusElement.GetString();
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest("Status field is required.");
        }



        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
