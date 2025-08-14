using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<CartItemResponse?> GetCartItemByIdAsync(int id);
        Task<IEnumerable<CartItemResponse>> GetAllCartItemsAsync();
        Task<CartItemResponse> CreateCartItemAsync(CreateCartItemRequest cartItemDto);

        Task DeleteCartItemAsync(int id);
        Task<IEnumerable<CartItemResponse>> GetCartItemsByUserIdAsync(int userId);
        Task<List<CartSummaryResponse>> GetCartSummaryByUserAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId, int restaurantId);
        Task<CouponApplicationResultResponse> GetCartTotalWithCouponAsync(int userId, int restaurantId, string couponCode);
        Task UpdateCartItemQuantityAsync(int id, int newQuantity);
        Task UpdateCartItemAsync(int id, CartItemResponse cartItemDto);
        Task<CartItemResponse?> AddOrUpdateCartItemAsync(int userId, int menuItemId, int quantity = 1);
        Task<CartItemResponse?> RemoveOrDecrementCartItemAsync(int userId, int menuItemId, int quantity = 1);
        Task ClearCartAsync(int userId, int restaurantId);
    }
}
