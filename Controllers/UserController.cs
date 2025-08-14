using AutoMapper;
using FoodDeliveryBackend.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using FoodDeliveryBackend.DTOs.Requests.Auth;
using FoodDeliveryBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

using System.Text.Json;
using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers([FromQuery] PaginationParams paginationParams)
        {
            var pagedUsers = await _userService.GetAllUsersAsync(paginationParams);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new {
                pagedUsers.CurrentPage,
                pagedUsers.PageSize,
                pagedUsers.TotalCount,
                pagedUsers.TotalPages,
                pagedUsers.HasNext,
                pagedUsers.HasPrevious
            }));

            return Ok(pagedUsers);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById([FromRoute] int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpGet("{id}/addresses")]
        public async Task<IActionResult> GetAddressesByUserId([FromRoute] int id)
        {
            var addresses = await _userService.GetAddressesByUserIdAsync(id);
            return Ok(addresses);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UserResponse userDto)
        {
            await _userService.UpdateUserAsync(id, userDto);
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("dashboard")]
        public ActionResult GetDashboardData()
        {
            // Placeholder for dashboard data
            return Ok(new { message = "Dashboard data loaded successfully!", userStats = 123, recentActivity = "No recent activity." });
        }

    }

}