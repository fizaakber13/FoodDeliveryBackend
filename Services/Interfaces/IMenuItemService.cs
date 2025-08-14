using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<MenuItemResponse?> GetMenuItemByIdAsync(int id);
        Task<PagedList<MenuItemResponse>> GetMenuItemsAsync(PaginationParams paginationParams);
        Task<IEnumerable<MenuItemResponse>> GetAllMenuItemsAsync();
        Task<MenuItemResponse> CreateMenuItemAsync(CreateMenuItemRequest menuItemDto);
        Task UpdateMenuItemAsync(int id, MenuItemResponse menuItemDto);
        Task DeleteMenuItemAsync(int id);
        Task<IEnumerable<MenuItemResponse>> GetMenuItemsByRestaurantIdAsync(int restaurantId);
        Task<IEnumerable<string>> GetMenuItemNamesAsync();
        Task<IEnumerable<MenuItemResponse>> SearchMenuItemsAsync(string name);
    }
}

