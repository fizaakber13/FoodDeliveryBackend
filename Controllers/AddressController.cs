using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet(Name = "GetAddressesV1")]
        [MapToApiVersion("1.0")]
        [SwaggerOperationSummary("Gets all addresses")]
        public async Task<ActionResult<IEnumerable<AddressResponse>>> GetAddressesV1()
        {
            var addresses = await _addressService.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpGet(Name = "GetAddressesV2")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<AddressResponse>>> GetAddressesV2()
        {
            //same methods given for both versions, given for understanding
            var addresses = await _addressService.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddressResponse>> GetAddress([FromRoute] int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null) return NotFound();
            return address;
        }

        [HttpPost]
        public async Task<ActionResult<AddressResponse>> AddAddress([FromBody] CreateAddressRequest addressDto)
        {
            var newAddress = await _addressService.CreateAddressAsync(addressDto);
            return CreatedAtAction(nameof(GetAddress), new { id = newAddress.Id }, newAddress);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress([FromRoute] int id, [FromBody] AddressResponse addressDto)
        {
            if (id != addressDto.Id)
                return BadRequest();

            await _addressService.UpdateAddressAsync(id, addressDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id)
        {
            await _addressService.DeleteAddressAsync(id);
            return NoContent();
        }
    }
}
