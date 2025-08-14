using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;


using System.Text.Json;
using FoodDeliveryBackend.Pagination;


namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders([FromQuery] PaginationParams paginationParams)
        {
            var pagedOrders = await _orderService.GetAllOrdersAsync(paginationParams);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new {
                pagedOrders.CurrentPage,
                pagedOrders.PageSize,
                pagedOrders.TotalCount,
                pagedOrders.TotalPages,
                pagedOrders.HasNext,
                pagedOrders.HasPrevious
            }));

            return Ok(pagedOrders);
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrder([FromRoute] int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId([FromRoute] int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
        
        [HttpGet("user/{userId}/search")]
        public async Task<IActionResult> SearchOrdersByRestaurant([FromRoute] int userId, [FromQuery] string restaurantName = "")
        {
            var orders = await _orderService.SearchOrdersByRestaurantAsync(userId, restaurantName);
            return Ok(orders);
        }


        [HttpPost]
        public async Task<ActionResult<OrderResponse>> AddOrder([FromBody] CreateOrderRequest orderDto)
        {
            try
            {
                var newOrder = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ General Error: " + ex.Message);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }



       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, [FromBody] string status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return NoContent();
        }



        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
