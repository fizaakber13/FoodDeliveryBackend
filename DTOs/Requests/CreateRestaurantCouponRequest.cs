using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateRestaurantCouponRequest(
        [Required]
        int RestaurantId,
        [Required]
        int CouponId
    );
}