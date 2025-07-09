using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Data;
using FoodDeliveryBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CouponController(AppDbContext ctx) => _context = ctx;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons() =>
          await _context.Coupons.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(int id) =>
          await _context.Coupons.FindAsync(id) switch
          {
              null => NotFound(),
              var coupon => coupon
          };

        [HttpPost]
        public async Task<ActionResult<Coupon>> AddCoupon(Coupon coupon)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (coupon.Condition == "date" && !coupon.ExpirationDate.HasValue)
                    return BadRequest("Expiration date is required for condition type 'date'.");

                if (coupon.Condition == "minPrice" && !coupon.MinOrderAmount.HasValue)
                    return BadRequest("Minimum order amount is required for condition type 'minPrice'.");

                if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < DateTime.Now)
                    return BadRequest("Date must be in the future.");

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCoupon), new { id = coupon.Id }, coupon);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Database error", error = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, Coupon coupon)
        {
            if (id != coupon.Id)
                return BadRequest("Coupon ID mismatch.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Validation failed", errors });
            }

            if (coupon.Condition == "date" && !coupon.ExpirationDate.HasValue)
                return BadRequest("Expiration date is required for condition type 'date'.");

            if (coupon.Condition == "minPrice" && !coupon.MinOrderAmount.HasValue)
                return BadRequest("Minimum order amount is required for condition type 'minPrice'.");

            if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < DateTime.Now)
                return BadRequest("Date must be in the future.");

            _context.Entry(coupon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update failed", error = ex.Message });
            }
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
