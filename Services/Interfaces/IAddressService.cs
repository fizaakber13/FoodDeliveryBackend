using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IAddressService
    {
        Task<AddressResponse?> GetAddressByIdAsync(int id);
        Task<IEnumerable<AddressResponse>> GetAllAddressesAsync();
        Task<AddressResponse> CreateAddressAsync(CreateAddressRequest addressDto);
        Task UpdateAddressAsync(int id, AddressResponse addressDto);
        Task DeleteAddressAsync(int id);
        Task<IEnumerable<AddressResponse>> GetAddressesByUserIdAsync(int userId);
    }
}
