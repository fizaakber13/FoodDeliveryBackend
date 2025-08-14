using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateUserRequest(
        [Required]
        [StringLength(100)]
        string? Name,

        [Required]
        [EmailAddress]
        string Email
    );
}