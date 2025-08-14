using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using FoodDeliveryBackend.DTOs.Requests.Auth;
using FoodDeliveryBackend.DTOs.Responses.Auth;
using System.Security.Claims;


using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<PagedList<UserResponse>> GetAllUsersAsync(PaginationParams paginationParams)
        {
            var source = _unitOfWork.Users.GetAllAsQueryable();

            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                source = source.Where(u => u.Name.ToLower().Contains(paginationParams.SearchTerm.ToLower()) ||
                                           u.Email.ToLower().Contains(paginationParams.SearchTerm.ToLower()));
            }

            var pagedList = await PagedList<User>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);

            var userResponses = pagedList.Select(user => new UserResponse(user.Id, user.Name, user.Email)).ToList();

            return new PagedList<UserResponse>(userResponses, pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize);
        }

        

        

        public async Task UpdateUserAsync(int id, UserResponse userDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return;

            user.Name = userDto.Name ?? user.Name;
            user.Email = userDto.Email;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return;

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserResponse?> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user == null) return null;
            return new UserResponse(user.Id, user.Name, user.Email);
        }

        public async Task<bool> SendOtpAsync(string email)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                return false;
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntry = new OtpEntry { Email = email, Otp = otp, CreatedAt = DateTime.UtcNow };

            _= _unitOfWork.OtpEntries.AddAsync(otpEntry);
            await _unitOfWork.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ZUPPR - Food & Beverage", "fizaakber13@gmail.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain") { Text = $"Your OTP code is: {otp}" };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("fizaakber13@gmail.com", "yudzqupldfvbakkp");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            return true;
        }

        public async Task<bool> RegisterSendOtpAsync(string email, string name)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                return false; // User already exists
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntry = new OtpEntry { Email = email, Otp = otp, CreatedAt = DateTime.UtcNow };

            _= _unitOfWork.OtpEntries.AddAsync(otpEntry);
            await _unitOfWork.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ZUPPR - Food & Beverage", "fizaakber13@gmail.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your OTP Code for Registration";
            message.Body = new TextPart("plain") { Text = $"Your OTP code is: {otp}" };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("fizaakber13@gmail.com", "yudzqupldfvbakkp");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            return true;
        }

        public async Task<TokenResponse?> VerifyOtpAsync(string email, string otp, string? name)
        {
            var latestOtp = (await _unitOfWork.OtpEntries.GetAsync(e => e.Email.ToLower() == email.ToLower()))
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault();

            if (latestOtp == null || latestOtp.Otp != otp) return null;

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            // If user doesn't exist, create them (this handles registration flow)
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    Name = name ?? email, // Use provided name or default to email
                    Role = "User" // Default role
                };
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            Console.WriteLine($"Generated Access Token: {accessToken}");
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new TokenResponse(accessToken, refreshToken);
        }
        
        public async Task<IEnumerable<AddressResponse>> GetAddressesByUserIdAsync(int userId)
        {
            var addresses = await _unitOfWork.Addresses.GetAllAsync(); // Assuming GetAllAsync() can be filtered later or a new method is added to IRepository
            return addresses.Where(a => a.UserId == userId).Select(a => new AddressResponse(a.Id, a.Line, a.IsDefault, a.Label, a.UserId));
        }

        public async Task UpdateUserRefreshTokenAsync(User user)
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}