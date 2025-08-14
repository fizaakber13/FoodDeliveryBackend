using System;

namespace FoodDeliveryBackend.DTOs.Responses
{
    public record CouponResponse(int Id, string Code, decimal DiscountAmount, string DiscountType, string Condition, decimal? MinOrderAmount, DateTime? ExpirationDate, bool IsActive);
    
}
