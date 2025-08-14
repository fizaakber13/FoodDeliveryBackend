using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateRestaurantRegistrationRequest(
        [Required]
        [StringLength(100)]
        string RestaurantName,

        [Required]
        [EmailAddress]
        [StringLength(150)]
        string Email,

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact must be a 10-digit number.")]
        string Contact,

        [Required]
        [StringLength(200)]
        string Location
    );
}