using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            var otp = new Random().Next(100000, 999999).ToString();

            var otpEntry = new OtpEntry
            {
                Email = request.Email,
                Otp = otp,
                CreatedAt = DateTime.UtcNow
            };

            _context.OtpRequests.Add(otpEntry);
            await _context.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ZUPPR - Food & Beverage", "fizaakber13@gmail.com"));
            message.To.Add(MailboxAddress.Parse(request.Email));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain") { Text = $"Your OTP code is: {otp}" };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("fizaakber13@gmail.com", "yudzqupldfvbakkp"); // App password
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return Ok(new { Message = "OTP sent successfully." });
        }

        
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerification request)
        {
            var email = request.Email?.Trim().ToLower();
            var otp = request.Otp?.Trim();
            var mode = request.Mode?.ToLower();

            var latestOtp = await _context.OtpRequests
                .Where(e => e.Email.ToLower() == email)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestOtp == null)
                return BadRequest(new { message = "No OTP sent to this email." });

            if (latestOtp.Otp != otp)
                return BadRequest(new { message = "Invalid OTP." });

            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailOrPhone.ToLower() == email);
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Email.ToLower() == email);

            if (mode == "login")
            {
                if (user == null && restaurant == null)
                    return NotFound(new { message = "Account not found. Please sign up first." });
            }


            return Ok(new
            {
                message = "OTP verified successfully.",
                isAdmin = user?.IsAdmin ?? false,
                id = user?.Id ?? restaurant?.Id ?? 0,  // Avoid null exception
                name = user?.Name ?? restaurant?.Name ?? string.Empty,  // Avoid null exception
                isRestaurant = restaurant != null
            });




        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.EmailOrPhone) || string.IsNullOrWhiteSpace(user.Name))
                return BadRequest("Name and email are required.");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.EmailOrPhone == user.EmailOrPhone);
            if (existingUser != null)
                return BadRequest("User already exists. Please log in.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Account created successfully.",
                id = user.Id,
                name = user.Name,
                isAdmin = user.IsAdmin
            });

        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.CartItems)
                .Include(u => u.Orders)
                .ToListAsync();

            return Ok(users);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.CartItems)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpGet("{id}/addresses")]
        public async Task<IActionResult> GetAddressesByUserId(int id)
        {
            var addresses = await _context.Addresses
                                          .Where(a => a.UserId == id)
                                          .ToListAsync();
            return Ok(addresses);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            
            user.Name = updatedUser.Name;
            user.EmailOrPhone = updatedUser.EmailOrPhone;
            user.IsAdmin = updatedUser.IsAdmin;

            
            var existingAddresses = _context.Addresses.Where(a => a.UserId == user.Id);
            _context.Addresses.RemoveRange(existingAddresses);

            
            foreach (var addr in updatedUser.Addresses)
            {
                _context.Addresses.Add(new Address
                {
                    Label = addr.Label,
                    Line = addr.Line,
                    IsDefault = addr.IsDefault,
                    UserId = user.Id
                });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.CartItems)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.EmailOrPhone.ToLower() == email.ToLower());

            if (user == null)
                return NotFound();

            return Ok(user);
        }

    }

}
