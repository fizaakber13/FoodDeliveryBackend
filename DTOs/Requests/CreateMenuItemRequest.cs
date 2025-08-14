using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateMenuItemRequest(
        [Required]
        [StringLength(100)]
        string Name,

        [StringLength(50)]
        string Category,

        [StringLength(255)]
        string Description,

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        double Price,

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        double Rating,

        [Required]
        int RestaurantId
    );
}