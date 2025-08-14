using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MenuItemResponse?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(id);
            if (menuItem == null) return null;
            return new MenuItemResponse(menuItem.Id, menuItem.Name, menuItem.Category, menuItem.Description, (double)menuItem.Price, menuItem.Rating, menuItem.RestaurantId);
        }

        public async Task<PagedList<MenuItemResponse>> GetMenuItemsAsync(PaginationParams paginationParams)
        {
            var source = _unitOfWork.MenuItems.GetAllAsQueryable();
            var pagedList = await PagedList<MenuItem>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);

            var menuItemDtos = pagedList.Select(mi => new MenuItemResponse(mi.Id, mi.Name, mi.Category, mi.Description, (double)mi.Price, mi.Rating, mi.RestaurantId)).ToList();

            return new PagedList<MenuItemResponse>(menuItemDtos, pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize);
        }

        public async Task<IEnumerable<MenuItemResponse>> GetAllMenuItemsAsync()
        {
            var menuItems = await _unitOfWork.MenuItems.GetAllAsync();
            return menuItems.Select(mi => new MenuItemResponse(mi.Id, mi.Name, mi.Category, mi.Description, (double)mi.Price, mi.Rating, mi.RestaurantId));
        }

        public async Task<MenuItemResponse> CreateMenuItemAsync(CreateMenuItemRequest menuItemDto)
        {
            var menuItem = new MenuItem
            {
                Name = menuItemDto.Name,
                Category = menuItemDto.Category,
                Description = menuItemDto.Description,
                Price = (decimal)menuItemDto.Price,
                Rating = menuItemDto.Rating,
                RestaurantId = menuItemDto.RestaurantId
            };
            await _unitOfWork.MenuItems.AddAsync(menuItem);
            await _unitOfWork.SaveChangesAsync();
            return new MenuItemResponse(menuItem.Id, menuItem.Name, menuItem.Category, menuItem.Description, (double)menuItem.Price, menuItem.Rating, menuItem.RestaurantId);
        }

        public async Task UpdateMenuItemAsync(int id, MenuItemResponse menuItemDto)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(id);
            if (menuItem == null) return;

            menuItem.Name = menuItemDto.Name;
            menuItem.Category = menuItemDto.Category;
            menuItem.Description = menuItemDto.Description;
            menuItem.Price = (decimal)menuItemDto.Price;
            menuItem.Rating = menuItemDto.Rating;
            menuItem.RestaurantId = menuItemDto.RestaurantId;

            _unitOfWork.MenuItems.Update(menuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(id);
            if (menuItem == null) return;

            _unitOfWork.MenuItems.Remove(menuItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItemResponse>> GetMenuItemsByRestaurantIdAsync(int restaurantId)
        {
            var menuItems = await _unitOfWork.MenuItems.FindAsync(mi => mi.RestaurantId == restaurantId);
            return menuItems.Select(mi => new MenuItemResponse(mi.Id, mi.Name, mi.Category, mi.Description, (double)mi.Price, mi.Rating, mi.RestaurantId));
        }

        public async Task<IEnumerable<string>> GetMenuItemNamesAsync()
        {
            return (await _unitOfWork.MenuItems.GetAllAsync()).Select(m => m.Name).Distinct().ToList();
        }

        public async Task<IEnumerable<MenuItemResponse>> SearchMenuItemsAsync(string name)
        {
            var items = await _unitOfWork.MenuItems.FindAsync(m => m.Name.ToLower().Contains(name.ToLower()));
            return items.Select(mi => new MenuItemResponse(mi.Id, mi.Name, mi.Category, mi.Description, (double)mi.Price, mi.Rating, mi.RestaurantId));
        }
    }
}