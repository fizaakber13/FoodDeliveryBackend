using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class RestaurantCouponService : IRestaurantCouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestaurantCouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RestaurantCouponResponse?> GetRestaurantCouponByIdAsync(int id)
        {
            var restaurantCoupon = await _unitOfWork.RestaurantCoupons.GetByIdAsync(id);
            if (restaurantCoupon == null) return null;
            return new RestaurantCouponResponse(restaurantCoupon.Id, restaurantCoupon.RestaurantId, restaurantCoupon.CouponId);
        }

        public async Task<IEnumerable<RestaurantCouponResponse>> GetAllRestaurantCouponsAsync()
        {
            var restaurantCoupons = await _unitOfWork.RestaurantCoupons.GetAllAsync();
            return restaurantCoupons.Select(rc => new RestaurantCouponResponse(rc.Id, rc.RestaurantId, rc.CouponId));
        }

        public async Task<RestaurantCouponResponse> CreateRestaurantCouponAsync(CreateRestaurantCouponRequest restaurantCouponDto)
        {
            var restaurantCoupon = new RestaurantCoupon
            {
                RestaurantId = restaurantCouponDto.RestaurantId,
                CouponId = restaurantCouponDto.CouponId
            };
            await _unitOfWork.RestaurantCoupons.AddAsync(restaurantCoupon);
            await _unitOfWork.SaveChangesAsync();
            return new RestaurantCouponResponse(restaurantCoupon.Id, restaurantCoupon.RestaurantId, restaurantCoupon.CouponId);
        }

        public async Task UpdateRestaurantCouponAsync(int id, RestaurantCouponResponse restaurantCouponDto)
        {
            var restaurantCoupon = await _unitOfWork.RestaurantCoupons.GetByIdAsync(id);
            if (restaurantCoupon == null) return;

            restaurantCoupon.RestaurantId = restaurantCouponDto.RestaurantId;
            restaurantCoupon.CouponId = restaurantCouponDto.CouponId;

            _unitOfWork.RestaurantCoupons.Update(restaurantCoupon);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRestaurantCouponAsync(int id)
        {
            var restaurantCoupon = await _unitOfWork.RestaurantCoupons.GetByIdAsync(id);
            if (restaurantCoupon == null) return;

            _unitOfWork.RestaurantCoupons.Remove(restaurantCoupon);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CouponApplicationResultResponse> ApplyCouponAsync(CouponApplyRequest request)
        {
            var coupon = await _unitOfWork.RestaurantCoupons.FirstOrDefaultAsync(
                rc => rc.RestaurantId == request.RestaurantId && rc.Coupon != null && rc.Coupon.Code == request.CouponCode,
                rc => rc.Coupon
            );

            if (coupon == null || coupon.Coupon == null)
            {
                return new CouponApplicationResultResponse(false, "Coupon not valid for this restaurant.", null, null, null);
            }

            var actualCoupon = coupon.Coupon;

            if (!coupon.Coupon.IsActive)
            {
                return new CouponApplicationResultResponse(false, "Coupon is inactive.", null, null, null);
            }

            if (coupon.Coupon.ExpirationDate < DateTime.UtcNow)
            {
                return new CouponApplicationResultResponse(false, "Coupon has expired.", null, null, null);
            }

            if (request.CartTotal < coupon.Coupon.MinOrderAmount)
            {
                return new CouponApplicationResultResponse(false, $"Minimum order amount is {coupon.Coupon.MinOrderAmount} to use this coupon.", null, null, null);
            }

            return new CouponApplicationResultResponse(
                true,
                "Coupon applied successfully.",
                actualCoupon.Code,
                actualCoupon.DiscountAmount,
                actualCoupon.DiscountType
            );
        }
    }
}