using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
        {
            return await _context.MenuItems.Include(m => m.Restaurant).ToListAsync();
        }

        [HttpGet("names")]
        public async Task<ActionResult<IEnumerable<string>>> GetMenuItemNames()
        {
            return await _context.MenuItems
                                 .Select(m => m.Name)
                                 .Distinct()
                                 .ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> SearchMenuItems([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name query is required.");

            var items = await _context.MenuItems
                                      .Where(m => m.Name.ToLower().Contains(name.ToLower()))
                                      .Include(m => m.Restaurant)
                                      .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                                         .Include(m => m.Restaurant)
                                         .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                return NotFound();

            return menuItem;
        }

        [HttpPost]
        public async Task<ActionResult<MenuItem>> AddMenuItem(MenuItem menuItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (menuItem.Rating < 0 || menuItem.Rating > 5)
                return BadRequest("Rating must be between 0 and 5.");

            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == menuItem.RestaurantId);
            if (!restaurantExists)
                return BadRequest($"Restaurant with ID {menuItem.RestaurantId} does not exist.");

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItem);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, MenuItem menuItem)
        {
            if (id != menuItem.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (menuItem.Rating < 0 || menuItem.Rating > 5)
                return BadRequest("Rating must be between 0 and 5.");

            _context.Entry(menuItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("by-restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItemsByRestaurant(int restaurantId)
        {
            var items = await _context.MenuItems
                                      .Where(m => m.RestaurantId == restaurantId)
                                      .Include(m => m.Restaurant)
                                      .ToListAsync();

            if (items == null || !items.Any())
                return NotFound("No menu items found for this restaurant.");

            return Ok(items);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
