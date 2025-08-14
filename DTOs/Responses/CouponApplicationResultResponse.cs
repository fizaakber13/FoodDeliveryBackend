using FoodDeliveryBackend.Models;

namespace FoodDeliveryBackend.DTOs.Responses
{
    public record CouponApplicationResultResponse(bool Success, string Message, string? Code, decimal? DiscountAmount, DiscountType? DiscountType);
}