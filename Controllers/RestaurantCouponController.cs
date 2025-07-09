using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    public class CouponApplyRequest
    {
        public int RestaurantId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public decimal CartTotal { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantCouponController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantCouponController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantCoupon>>> GetAll()
        {
            return await _context.RestaurantCoupons
                .Include(rc => rc.Restaurant)
                .Include(rc => rc.Coupon)
                .ToListAsync();
        }

        
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCouponsByRestaurantId(int restaurantId)
        {
            var coupons = await _context.RestaurantCoupons
                .Where(rc => rc.RestaurantId == restaurantId)
                .Include(rc => rc.Coupon)
                .Select(rc => rc.Coupon)
                .ToListAsync();

            return coupons;
        }

        
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CouponApplyRequest request)
        {
            var coupon = await _context.RestaurantCoupons
                .Where(rc => rc.RestaurantId == request.RestaurantId && rc.Coupon.Code == request.CouponCode)
                .Select(rc => rc.Coupon)
                .FirstOrDefaultAsync();

            if (coupon == null)
                return NotFound("Coupon not valid for this restaurant.");

            if (!coupon.IsActive)
                return BadRequest("Coupon is inactive.");

            if (coupon.ExpirationDate < DateTime.UtcNow)
                return BadRequest("Coupon has expired.");

            if (request.CartTotal < coupon.MinOrderAmount)
                return BadRequest($"Minimum order amount is {coupon.MinOrderAmount} to use this coupon.");

            return Ok(new
            {
                coupon.Code,
                coupon.DiscountAmount,
                coupon.DiscountType
            });
        }

        
        [HttpPost]
        public async Task<ActionResult<RestaurantCoupon>> Add(RestaurantCoupon rc)
        {
            _context.RestaurantCoupons.Add(rc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = rc.Id }, rc);
        }

       
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
