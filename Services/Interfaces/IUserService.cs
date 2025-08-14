using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.DTOs.Requests.Auth;
using FoodDeliveryBackend.DTOs.Responses.Auth;
using FoodDeliveryBackend.Models;

using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<PagedList<UserResponse>> GetAllUsersAsync(PaginationParams paginationParams);
        
        Task UpdateUserAsync(int id, UserResponse userDto);
        Task DeleteUserAsync(int id);
        Task<UserResponse?> GetUserByEmailAsync(string email);
        
        Task<bool> SendOtpAsync(string email);
        Task<bool> RegisterSendOtpAsync(string email, string name);
        Task<TokenResponse?> VerifyOtpAsync(string email, string otp, string? name);
        Task<IEnumerable<AddressResponse>> GetAddressesByUserIdAsync(int userId);
        Task UpdateUserRefreshTokenAsync(User user);
    }
}
