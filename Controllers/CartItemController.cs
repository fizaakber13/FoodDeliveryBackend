using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemResponse>>> GetCartItems()
        {
            var cartItems = await _cartItemService.GetAllCartItemsAsync();
            return Ok(cartItems);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CartItemResponse>> GetCartItem([FromRoute] int id)
        {
            var cartItem = await _cartItemService.GetCartItemByIdAsync(id);
            if (cartItem == null)
                return NotFound();

            return cartItem;
        }

        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItemResponse>>> GetCartItemsByUserId([FromRoute] int userId)
        {
            var cartItems = await _cartItemService.GetCartItemsByUserIdAsync(userId);
            return Ok(cartItems);
        }

        
        [HttpGet("summary/{userId}")]
        public async Task<ActionResult> GetCartSummaryByUser([FromRoute] int userId)
        {
            var groupedCart = await _cartItemService.GetCartSummaryByUserAsync(userId);
            return Ok(groupedCart);
        }

       
        [HttpGet("user/{userId}/restaurant/{restaurantId}/total")]
        public async Task<ActionResult<decimal>> GetCartTotal([FromRoute] int userId,[FromRoute] int restaurantId)
        {
            var total = await _cartItemService.GetCartTotalAsync(userId, restaurantId);
            return Ok(total);
        }

      
        [HttpGet("user/{userId}/restaurant/{restaurantId}/total-with-coupon")]
        public async Task<ActionResult> GetCartTotalWithCoupon([FromRoute] int userId,[FromRoute] int restaurantId, [FromQuery] string couponCode)
        {
            var result = await _cartItemService.GetCartTotalWithCouponAsync(userId, restaurantId, couponCode);
            return Ok(result);
        }

        
        [HttpPost]
        public async Task<ActionResult<CartItemResponse>> AddCartItem([FromBody] CreateCartItemRequest cartItemDto)
        {
            try
            {
                var newCartItem = await _cartItemService.CreateCartItemAsync(cartItemDto);
                return CreatedAtAction(nameof(GetCartItem), new { id = newCartItem.Id }, newCartItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("add-or-update")]
        public async Task<ActionResult<CartItemResponse>> AddOrUpdateCartItem([FromBody] AddOrUpdateCartItemRequest request)
        {
            try
            {
                var cartItem = await _cartItemService.AddOrUpdateCartItemAsync(request.UserId, request.MenuItemId, request.Quantity);
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("remove-or-decrement")]
        public async Task<ActionResult<CartItemResponse?>> RemoveOrDecrementCartItem([FromBody] RemoveOrDecrementCartItemRequest request)
        {
            try
            {
                var cartItem = await _cartItemService.RemoveOrDecrementCartItemAsync(request.UserId, request.MenuItemId, request.Quantity);
                if (cartItem == null)
                {
                    return NoContent(); // Item was fully removed
                }
                return Ok(cartItem); // Item quantity was decremented
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem([FromRoute] int id, [FromBody] CartItemResponse cartItemDto)
        {
            if (id != cartItemDto.Id)
                return BadRequest();

            await _cartItemService.UpdateCartItemAsync(id, cartItemDto);
            return NoContent();
        }

        
        [HttpPut("{id}/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromRoute] int id, [FromBody] int newQuantity)
        {
            await _cartItemService.UpdateCartItemQuantityAsync(id, newQuantity);
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] int id)
        {
            await _cartItemService.DeleteCartItemAsync(id);
            return NoContent();
        }

    }


}
