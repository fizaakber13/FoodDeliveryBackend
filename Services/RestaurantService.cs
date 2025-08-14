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
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestaurantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RestaurantResponse?> GetRestaurantByIdAsync(int id)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(id);
            if (restaurant == null) return null;
            return new RestaurantResponse(restaurant.Id, restaurant.Name, restaurant.Email, restaurant.Cuisine, restaurant.Address, restaurant.Rating, restaurant.Contact);
        }

        public async Task<PagedList<RestaurantResponse>> GetAllRestaurantsAsync(PaginationParams paginationParams)
        {
            var source = _unitOfWork.Restaurants.GetAllAsQueryable(); 

            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                source = source.Where(r => r.Name.ToLower().Contains(paginationParams.SearchTerm.ToLower()));
            }

            var pagedList = await PagedList<Restaurant>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);

            var restaurantResponses = pagedList.Select(r => new RestaurantResponse(r.Id, r.Name, r.Email, r.Cuisine, r.Address, r.Rating, r.Contact)).ToList();

            return new PagedList<RestaurantResponse>(restaurantResponses, pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize);
        }

        public async Task<RestaurantResponse> CreateRestaurantAsync(CreateRestaurantRequest restaurantDto)
        {
            var restaurant = new Restaurant
            {
                Name = restaurantDto.Name,
                Email = restaurantDto.Email,
                Cuisine = restaurantDto.Cuisine,
                Address = restaurantDto.Address,
                Rating = restaurantDto.Rating,
                Contact = restaurantDto.Contact
            };
            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
            return new RestaurantResponse(restaurant.Id, restaurant.Name, restaurant.Email, restaurant.Cuisine, restaurant.Address, restaurant.Rating, restaurant.Contact);
        }

        public async Task UpdateRestaurantAsync(int id, RestaurantResponse restaurantDto)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(id);
            if (restaurant == null) return;

            restaurant.Name = restaurantDto.Name;
            restaurant.Email = restaurantDto.Email;
            restaurant.Cuisine = restaurantDto.Cuisine;
            restaurant.Address = restaurantDto.Address;
            restaurant.Rating = restaurantDto.Rating;
            restaurant.Contact = restaurantDto.Contact;

            _unitOfWork.Restaurants.Update(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRestaurantAsync(int id)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(id);
            if (restaurant == null) return;

            _unitOfWork.Restaurants.Remove(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<RestaurantResponse?> GetRestaurantByEmailAsync(string email)
        {
            var restaurant = await _unitOfWork.Restaurants.FirstOrDefaultAsync(r => r.Email.ToLower() == email.ToLower());
            if (restaurant == null) return null;
            return new RestaurantResponse(restaurant.Id, restaurant.Name, restaurant.Email, restaurant.Cuisine, restaurant.Address, restaurant.Rating, restaurant.Contact);
        }

        public async Task<IEnumerable<string>> GetRestaurantNamesAsync()
        {
            return (await _unitOfWork.Restaurants.GetAllAsync()).Select(r => r.Name).Distinct().ToList();
        }

        public async Task<IEnumerable<string>> GetCuisinesAsync()
        {
            return (await _unitOfWork.Restaurants.GetAllAsync()).Select(r => r.Cuisine).Distinct().ToList();
        }

        public async Task<IEnumerable<double>> GetRatingsAsync()
        {
            return (await _unitOfWork.Restaurants.GetAllAsync()).Select(r => r.Rating).Distinct().ToList();
        }

        public async Task<IEnumerable<string>> GetAddressesAsync()
        {
            return (await _unitOfWork.Restaurants.GetAllAsync()).Select(r => r.Address).Distinct().ToList();
        }

        public async Task<IEnumerable<RestaurantResponse>> SearchRestaurantsAsync(string? name, string? cuisine, int? rating)
        {
            var query = await _unitOfWork.Restaurants.GetAsync();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrWhiteSpace(cuisine))
                query = query.Where(r => r.Cuisine == cuisine);

            if (rating.HasValue)
                query = query.Where(r => r.Rating >= rating.Value);

            var restaurants = query.ToList();
            return restaurants.Select(r => new RestaurantResponse(r.Id, r.Name, r.Email, r.Cuisine, r.Address, r.Rating, r.Contact));
        }
    }
}