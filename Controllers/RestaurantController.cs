using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using System.Text.Json;
using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantResponse>>> GetRestaurants([FromQuery] PaginationParams paginationParams)
        {
            var pagedRestaurants = await _restaurantService.GetAllRestaurantsAsync(paginationParams);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new {
                pagedRestaurants.CurrentPage,
                pagedRestaurants.PageSize,
                pagedRestaurants.TotalCount,
                pagedRestaurants.TotalPages,
                pagedRestaurants.HasNext,
                pagedRestaurants.HasPrevious
            }));

            return Ok(pagedRestaurants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantResponse>> GetRestaurantById([FromRoute] int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
                return NotFound();

            return Ok(restaurant);
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<RestaurantResponse>> GetByEmail([FromRoute] string email)
        {
            var rest = await _restaurantService.GetRestaurantByEmailAsync(email);
            if (rest == null) return NotFound();
            return Ok(rest);
        }

        [HttpPost]
        public async Task<ActionResult<RestaurantResponse>> AddRestaurant([FromBody] CreateRestaurantRequest restaurantDto)
        {
            

            var existingRestaurant = await _restaurantService.GetRestaurantByEmailAsync(restaurantDto.Email);
            if (existingRestaurant != null)
                return Conflict("Email already exists for another restaurant.");

            var newRestaurant = await _restaurantService.CreateRestaurantAsync(restaurantDto);

            return CreatedAtAction(nameof(GetRestaurantById), new { id = newRestaurant.Id }, newRestaurant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, [FromBody] RestaurantResponse restaurantDto)
        {
            if (id != restaurantDto.Id)
                return BadRequest("Restaurant ID mismatch");

            

            await _restaurantService.UpdateRestaurantAsync(id, restaurantDto);
            return Ok(restaurantDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
        {
            await _restaurantService.DeleteRestaurantAsync(id);
            return NoContent();
        }

        [HttpGet("names")]
        public async Task<ActionResult<IEnumerable<string>>> GetRestaurantNames()
        {
            return Ok(await _restaurantService.GetRestaurantNamesAsync());
        }

        [HttpGet("cuisines")]
        public async Task<ActionResult<IEnumerable<string>>> GetCuisines()
        {
            return Ok(await _restaurantService.GetCuisinesAsync());
        }

        [HttpGet("ratings")]
        public async Task<ActionResult<IEnumerable<double>>> GetRatings()
        {
            return Ok(await _restaurantService.GetRatingsAsync());
        }

        [HttpGet("addresses")]
        public async Task<ActionResult<IEnumerable<string>>> GetAddresses()
        {
            return Ok(await _restaurantService.GetAddressesAsync());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RestaurantResponse>>> SearchRestaurants(
            [FromQuery] string? name,
            [FromQuery] string? cuisine,
            [FromQuery] int? rating)
        {
            var restaurants = await _restaurantService.SearchRestaurantsAsync(name, cuisine, rating);
            return Ok(restaurants);
        }
    }
}
