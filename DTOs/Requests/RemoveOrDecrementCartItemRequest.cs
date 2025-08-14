using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record RemoveOrDecrementCartItemRequest(
        [Required]
        int UserId,
        [Required]
        int MenuItemId,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        int Quantity
    );
}