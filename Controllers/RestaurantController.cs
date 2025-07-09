using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            return await _context.Restaurants.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurantById(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
                return NotFound();

            return Ok(restaurant);
        }

        // ✅ GET by email (for login)
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<Restaurant>> GetByEmail(string email)
        {
            var rest = await _context.Restaurants.FirstOrDefaultAsync(r => r.Email == email);
            if (rest == null) return NotFound();
            return Ok(rest);
        }

        [HttpPost]
        public async Task<ActionResult<Restaurant>> AddRestaurant(Restaurant restaurant)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (restaurant.Rating < 0 || restaurant.Rating > 5)
                return BadRequest("Rating must be between 0 and 5.");

            // ✅ Check if email already exists
            if (await _context.Restaurants.AnyAsync(r => r.Email == restaurant.Email))
                return Conflict("Email already exists for another restaurant.");

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRestaurantById), new { id = restaurant.Id }, restaurant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, Restaurant updatedRestaurant)
        {
            if (id != updatedRestaurant.Id)
                return BadRequest("Restaurant ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatedRestaurant.Rating < 0 || updatedRestaurant.Rating > 5)
                return BadRequest("Rating must be between 0 and 5.");

            _context.Entry(updatedRestaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Restaurants.Any(r => r.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(updatedRestaurant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
                return NotFound();

            if (restaurant.MenuItems != null && restaurant.MenuItems.Any())
                _context.MenuItems.RemoveRange(restaurant.MenuItems);

            _context.Restaurants.Remove(restaurant);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting restaurant: {ex.Message}");
            }

            return NoContent();
        }

        [HttpGet("names")]
        public async Task<ActionResult<IEnumerable<string>>> GetRestaurantNames()
        {
            return await _context.Restaurants.Select(r => r.Name).Distinct().ToListAsync();
        }

        [HttpGet("cuisines")]
        public async Task<ActionResult<IEnumerable<string>>> GetCuisines()
        {
            return await _context.Restaurants.Select(r => r.Cuisine).Distinct().ToListAsync();
        }

        [HttpGet("ratings")]
        public async Task<ActionResult<IEnumerable<double>>> GetRatings()
        {
            return await _context.Restaurants.Select(r => r.Rating).Distinct().ToListAsync();
        }

        [HttpGet("addresses")]
        public async Task<ActionResult<IEnumerable<string>>> GetAddresses()
        {
            return await _context.Restaurants.Select(r => r.Address).Distinct().ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> SearchRestaurants(
            [FromQuery] string? name,
            [FromQuery] string? cuisine,
            [FromQuery] int? rating)
        {
            var query = _context.Restaurants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrWhiteSpace(cuisine))
                query = query.Where(r => r.Cuisine == cuisine);

            if (rating.HasValue)
                query = query.Where(r => r.Rating >= rating.Value);

            return Ok(await query.ToListAsync());
        }
    }
}
