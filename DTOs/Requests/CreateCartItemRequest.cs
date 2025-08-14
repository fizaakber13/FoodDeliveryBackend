using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateCartItemRequest(
        [Required]
        int UserId,
        [Required]
        int RestaurantId,
        [Required]
        int MenuItemId,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        int Quantity
    );
}