using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CouponApplyRequest(
        [Required]
        int RestaurantId,
        [Required]
        string CouponCode,
        [Required]
        decimal CartTotal
    );
}