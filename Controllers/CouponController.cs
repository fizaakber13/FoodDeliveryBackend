using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FoodDeliveryBackend.Services.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using System.Text.Json;
using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponResponse>>> GetCoupons([FromQuery] PaginationParams paginationParams)
        {
            var pagedCoupons = await _couponService.GetAllCouponsAsync(paginationParams);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new {
                pagedCoupons.CurrentPage,
                pagedCoupons.PageSize,
                pagedCoupons.TotalCount,
                pagedCoupons.TotalPages,
                pagedCoupons.HasNext,
                pagedCoupons.HasPrevious
            }));

            return Ok(pagedCoupons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponResponse>> GetCoupon([FromRoute] int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound();
            return coupon;
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<CouponResponse>>> GetCouponsByRestaurantId([FromRoute] int restaurantId)
        {
            var coupons = await _couponService.GetCouponsByRestaurantIdAsync(restaurantId);
            return Ok(coupons);
        }

        [HttpPost]
        public async Task<ActionResult<CouponResponse>> AddCoupon([FromBody] CreateCouponRequest couponDto)
        {
            try
            {
                

                var newCoupon = await _couponService.CreateCouponAsync(couponDto);

                return CreatedAtAction(nameof(GetCoupon), new { id = newCoupon.Id }, newCoupon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon([FromRoute] int id, [FromBody] CouponResponse couponDto)
        {
            if (id != couponDto.Id)
                return BadRequest("Coupon ID mismatch.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                await _couponService.UpdateCouponAsync(id, couponDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update failed", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon([FromRoute] int id)
        {
            await _couponService.DeleteCouponAsync(id);
            return NoContent();
        }
    }
}
