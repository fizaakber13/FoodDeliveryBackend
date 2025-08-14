using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FoodDeliveryBackend.Pagination;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemResponse>>> GetMenuItems([FromQuery] PaginationParams paginationParams)
        {
            var pagedMenuItems = await _menuItemService.GetMenuItemsAsync(paginationParams);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new {
                pagedMenuItems.CurrentPage,
                pagedMenuItems.PageSize,
                pagedMenuItems.TotalCount,
                pagedMenuItems.TotalPages,
                pagedMenuItems.HasNext,
                pagedMenuItems.HasPrevious
            }));

            return Ok(pagedMenuItems);
        }

        [HttpGet("names")]
        public async Task<ActionResult<IEnumerable<string>>> GetMenuItemNames()
        {
            return Ok(await _menuItemService.GetMenuItemNamesAsync());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MenuItemResponse>>> SearchMenuItems([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name query is required.");

            var items = await _menuItemService.SearchMenuItemsAsync(name);

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemResponse>> GetMenuItem([FromRoute] int id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
                return NotFound();

            return menuItem;
        }

        [HttpPost]
        public async Task<ActionResult<MenuItemResponse>> AddMenuItem([FromBody] CreateMenuItemRequest menuItemDto)
        {
            

            var newMenuItem = await _menuItemService.CreateMenuItemAsync(menuItemDto);

            return CreatedAtAction(nameof(GetMenuItem), new { id = newMenuItem.Id }, newMenuItem);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem([FromRoute] int id, [FromBody] MenuItemResponse menuItemDto)
        {
            if (id != menuItemDto.Id)
                return BadRequest();

            

            await _menuItemService.UpdateMenuItemAsync(id, menuItemDto);
            return NoContent();
        }
        [HttpGet("by-restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<MenuItemResponse>>> GetMenuItemsByRestaurant([FromRoute] int restaurantId)
        {
            var items = await _menuItemService.GetMenuItemsByRestaurantIdAsync(restaurantId);

            if (items == null || !items.Any())
                return NotFound("No menu items found for this restaurant.");

            return Ok(items);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem([FromRoute] int id)
        {
            await _menuItemService.DeleteMenuItemAsync(id);
            return NoContent();
        }
    }
}
