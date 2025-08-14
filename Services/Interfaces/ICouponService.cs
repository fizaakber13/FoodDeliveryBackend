using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface ICouponService
    {
        Task<CouponResponse?> GetCouponByIdAsync(int id);
        Task<PagedList<CouponResponse>> GetAllCouponsAsync(PaginationParams paginationParams);
        Task<CouponResponse> CreateCouponAsync(CreateCouponRequest couponDto);
        Task UpdateCouponAsync(int id, CouponResponse couponDto);
        Task DeleteCouponAsync(int id);
        Task<IEnumerable<CouponResponse>> GetCouponsByRestaurantIdAsync(int restaurantId);
    }
}
