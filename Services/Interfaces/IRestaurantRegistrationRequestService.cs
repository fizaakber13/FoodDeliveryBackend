using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IRestaurantRegistrationRequestService
    {
        Task<RestaurantRegistrationRequestResponse?> GetRestaurantRegistrationRequestByIdAsync(int id);
        Task<IEnumerable<RestaurantRegistrationRequestResponse>> GetAllRestaurantRegistrationRequestsAsync();
        Task<RestaurantRegistrationRequestResponse> CreateRestaurantRegistrationRequestAsync(CreateRestaurantRegistrationRequest requestDto);
        Task DeleteRestaurantRegistrationRequestAsync(int id);
        
    }
}
