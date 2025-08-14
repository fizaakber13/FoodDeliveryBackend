using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IRestaurantCouponService
    {
        Task<RestaurantCouponResponse?> GetRestaurantCouponByIdAsync(int id);
        Task<IEnumerable<RestaurantCouponResponse>> GetAllRestaurantCouponsAsync();
        Task<RestaurantCouponResponse> CreateRestaurantCouponAsync(CreateRestaurantCouponRequest restaurantCouponDto);
        Task UpdateRestaurantCouponAsync(int id, RestaurantCouponResponse restaurantCouponDto);
        Task DeleteRestaurantCouponAsync(int id);
        Task<CouponApplicationResultResponse> ApplyCouponAsync(CouponApplyRequest request);
    }
}
