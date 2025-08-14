using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RestaurantRegistrationController : ControllerBase
    {
        private readonly IRestaurantRegistrationRequestService _service;

        public RestaurantRegistrationController(IRestaurantRegistrationRequestService service)
        {
            _service = service;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestRegistration([FromBody] CreateRestaurantRegistrationRequest requestDto)
        {
            

            try
            {
                var newRequest = await _service.CreateRestaurantRegistrationRequestAsync(requestDto);
                return Ok(new { message = "Request submitted successfully. Admin will review it soon." });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message); // Assuming service throws specific exception for conflict
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantRegistrationRequestResponse>>> GetAllRequests()
        {
            return Ok(await _service.GetAllRestaurantRegistrationRequestsAsync());
        }

        
        
    }
}
