using Microsoft.AspNetCore.Mvc;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantRegistrationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantRegistrationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestRegistration(RestaurantRegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.RestaurantRegistrationRequests.AnyAsync(r => r.Email == request.Email))
                return Conflict("A request with this email has already been submitted.");

            _context.RestaurantRegistrationRequests.Add(request);
            await _context.SaveChangesAsync();

            await NotifyAdminsOfRequest(request);

            return Ok(new { message = "Request submitted successfully. Admin will review it soon." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantRegistrationRequest>>> GetAllRequests()
        {
            return await _context.RestaurantRegistrationRequests
                                 .OrderByDescending(r => r.RequestedAt)
                                 .ToListAsync();
        }

        
        private async Task NotifyAdminsOfRequest(RestaurantRegistrationRequest request)
        {
            var adminEmails = await _context.Users
                .Where(u => u.IsAdmin)
                .Select(u => u.EmailOrPhone)
                .ToListAsync();

            var messageText = $"New restaurant registration request:\n\n" +
                              $"Restaurant Name: {request.RestaurantName}\n" +
                              $"Email: {request.Email}\n" +
                              $"Contact: {request.Contact}\n" +
                              $"Location: {request.Location}\n" +
                              $"Time: {request.RequestedAt}\n";

            foreach (var adminEmail in adminEmails)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ZUPPR Admin:", "fizaakber13@gmail.com"));
                message.To.Add(MailboxAddress.Parse(adminEmail));
                message.Subject = "🔔 New Restaurant Registration Request";
                message.Body = new TextPart("plain") { Text = messageText };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("fizaakber13@gmail.com", "yudzqupldfvbakkp"); // Your Gmail App Password
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
