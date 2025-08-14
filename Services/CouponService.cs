using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private CouponResponse MapToResponse(Coupon coupon)
        {
            return new CouponResponse(
                coupon.Id,
                coupon.Code,
                coupon.DiscountAmount,
                coupon.DiscountType.ToString(),
                coupon.Condition,
                coupon.MinOrderAmount,
                coupon.ExpirationDate,
                coupon.IsActive
            );
        }

        public async Task<CouponResponse?> GetCouponByIdAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            return coupon == null ? null : MapToResponse(coupon);
        }

        public async Task<PagedList<CouponResponse>> GetAllCouponsAsync(PaginationParams paginationParams)
        {
            var source = _unitOfWork.Coupons.GetAllAsQueryable();

            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                source = source.Where(c => c.Code.ToLower().Contains(paginationParams.SearchTerm.ToLower()) ||
                                           c.Condition.ToLower().Contains(paginationParams.SearchTerm.ToLower()));
            }

            var pagedList = await PagedList<Coupon>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);

            var couponResponses = pagedList.Select(MapToResponse).ToList();

            return new PagedList<CouponResponse>(couponResponses, pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize);
        }

        public async Task<CouponResponse> CreateCouponAsync(CreateCouponRequest couponDto)
        {
            var coupon = new Coupon
            {
                Code = couponDto.Code,
                DiscountAmount = couponDto.DiscountAmount,
                DiscountType = (Models.DiscountType)Enum.Parse(typeof(Models.DiscountType), couponDto.DiscountType),
                Condition = couponDto.Condition,
                MinOrderAmount = couponDto.MinOrderAmount,
                ExpirationDate = couponDto.ExpirationDate,
                IsActive = couponDto.IsActive
            };
            await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            return MapToResponse(coupon);
        }

        public async Task UpdateCouponAsync(int id, CouponResponse couponDto)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null) return;

            coupon.Code = couponDto.Code;
            coupon.DiscountAmount = couponDto.DiscountAmount;
            coupon.DiscountType = (Models.DiscountType)Enum.Parse(typeof(Models.DiscountType), couponDto.DiscountType);
            coupon.Condition = couponDto.Condition;
            coupon.MinOrderAmount = couponDto.MinOrderAmount;
            coupon.ExpirationDate = couponDto.ExpirationDate;
            coupon.IsActive = couponDto.IsActive;

            _unitOfWork.Coupons.Update(coupon);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCouponAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null) return;

            _unitOfWork.Coupons.Remove(coupon);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CouponResponse>> GetCouponsByRestaurantIdAsync(int restaurantId)
        {
            var restaurantCoupons = await _unitOfWork.RestaurantCoupons.GetAsync(
                rc => rc.RestaurantId == restaurantId,
                includeProperties: rc => rc.Coupon
            );

            return restaurantCoupons
                .Where(rc => rc.Coupon != null && rc.Coupon.IsActive && rc.Coupon.ExpirationDate > DateTime.UtcNow)
                .Select(rc => new CouponResponse(
                    rc.Coupon!.Id,
                    rc.Coupon.Code,
                    rc.Coupon.DiscountAmount,
                    rc.Coupon.DiscountType.ToString(),
                    rc.Coupon.Condition,
                    rc.Coupon.MinOrderAmount,
                    rc.Coupon.ExpirationDate,
                    rc.Coupon.IsActive
                ));
        }
    }
}