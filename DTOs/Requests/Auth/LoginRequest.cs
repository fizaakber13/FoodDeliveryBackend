using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests.Auth
{
    public record LoginRequest
    (
        [Required]
        [EmailAddress]
        string Email
        
    );
}
