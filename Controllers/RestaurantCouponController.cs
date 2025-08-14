using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RestaurantCouponController : ControllerBase
    {
        private readonly IRestaurantCouponService _restaurantCouponService;
        private readonly ICouponService _couponService;

        public RestaurantCouponController(IRestaurantCouponService restaurantCouponService, ICouponService couponService)
        {
            _restaurantCouponService = restaurantCouponService;
            _couponService = couponService;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantCouponResponse>>> GetAll()
        {
            var restaurantCoupons = await _restaurantCouponService.GetAllRestaurantCouponsAsync();
            return Ok(restaurantCoupons);
        }

        
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CouponApplyRequest request)
        {
            var result = await _restaurantCouponService.ApplyCouponAsync(request);
            if (result.GetType().GetProperty("Message") != null) // Check if it's an error message
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        
        [HttpPost]
        public async Task<ActionResult<RestaurantCouponResponse>> Add([FromBody] CreateRestaurantCouponRequest restaurantCouponDto)
        {
            var newRestaurantCoupon = await _restaurantCouponService.CreateRestaurantCouponAsync(restaurantCouponDto);
            return CreatedAtAction(nameof(GetAll), new { id = newRestaurantCoupon.Id }, newRestaurantCoupon);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _restaurantCouponService.DeleteRestaurantCouponAsync(id);
            return NoContent();
        }
    }
}
