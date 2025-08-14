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
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        // GET code
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemResponse>>> GetOrderItems()
        {
            var orderItems = await _orderItemService.GetAllOrderItemsAsync();
            return Ok(orderItems);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemResponse>> GetOrderItem([FromRoute] int id)
        {
            var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            return orderItem;
        }

        // POST code
        [HttpPost]
        public async Task<ActionResult<OrderItemResponse>> AddOrderItem([FromBody] CreateOrderItemRequest orderItemDto)
        {
            var newOrderItem = await _orderItemService.CreateOrderItemAsync(orderItemDto);
            return CreatedAtAction(nameof(GetOrderItem), new { id = newOrderItem.Id }, newOrderItem);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem([FromRoute] int id, [FromBody] OrderItemResponse orderItemDto)
        {
            if (id != orderItemDto.Id)
            {
                return BadRequest();
            }

            await _orderItemService.UpdateOrderItemAsync(id, orderItemDto);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem([FromRoute] int id)
        {
            await _orderItemService.DeleteOrderItemAsync(id);
            return NoContent();
        }
    }
}
