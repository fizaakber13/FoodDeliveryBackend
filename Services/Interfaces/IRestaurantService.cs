using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<RestaurantResponse?> GetRestaurantByIdAsync(int id);
        Task<PagedList<RestaurantResponse>> GetAllRestaurantsAsync(PaginationParams paginationParams);
        Task<RestaurantResponse> CreateRestaurantAsync(CreateRestaurantRequest restaurantDto);
        Task UpdateRestaurantAsync(int id, RestaurantResponse restaurantDto);
        Task DeleteRestaurantAsync(int id);
        Task<RestaurantResponse?> GetRestaurantByEmailAsync(string email);
        Task<IEnumerable<string>> GetRestaurantNamesAsync();
        Task<IEnumerable<string>> GetCuisinesAsync();
        Task<IEnumerable<double>> GetRatingsAsync();
        Task<IEnumerable<string>> GetAddressesAsync();
        Task<IEnumerable<RestaurantResponse>> SearchRestaurantsAsync(string? name, string? cuisine, int? rating);
    }
}
