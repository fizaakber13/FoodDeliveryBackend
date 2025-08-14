using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests.Auth
{
    public record OtpSendRequest
    (
        [Required]
        [EmailAddress]
        string Email
    );
}
