using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses.Include(a => a.User).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses.Include(a => a.User)
                                                  .FirstOrDefaultAsync(a => a.Id == id);
            if (address == null) return NotFound();
            return address;
        }

        [HttpPost]
        public async Task<ActionResult<Address>> AddAddress(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, Address updatedAddress)
        {
            if (id != updatedAddress.Id)
                return BadRequest();

            _context.Entry(updatedAddress).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
