using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantCouponController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantCouponController(AppDbContext context)
        {
            _context = context;
        }

        // GET code
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantCoupon>>> GetAll()
        {
            return await _context.RestaurantCoupons
                .Include(rc => rc.Restaurant)
                .Include(rc => rc.Coupon)
                .ToListAsync();
        }

        // POST code
        [HttpPost]
        public async Task<ActionResult<RestaurantCoupon>> Add(RestaurantCoupon rc)
        {
            _context.RestaurantCoupons.Add(rc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = rc.Id }, rc);
        }

        // DELETE code
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rc = await _context.RestaurantCoupons.FindAsync(id);
            if (rc == null)
                return NotFound();

            _context.RestaurantCoupons.Remove(rc);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
