using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateRestaurantRequest(
        [Required]
        [StringLength(100)]
        string Name,

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,

        [Required]
        [StringLength(50)]
        string Cuisine,

        [Required]
        [StringLength(255)]
        string Address,

        [Required]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        double Rating,

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits.")]
        string Contact
    );
}