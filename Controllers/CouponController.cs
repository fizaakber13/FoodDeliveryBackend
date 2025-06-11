using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CouponController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            return coupon;
        }

        [HttpPost]
        public async Task<ActionResult<Coupon>> AddCoupon(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCoupon), new { id = coupon.Id }, coupon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, Coupon updatedCoupon)
        {
            if (id != updatedCoupon.Id)
                return BadRequest();

            _context.Entry(updatedCoupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
