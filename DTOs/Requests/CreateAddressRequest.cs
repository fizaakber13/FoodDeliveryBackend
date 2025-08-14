using FoodDeliveryBackend.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateAddressRequest(
        [Required]
        [NotNullOrWhitespace]
        string Line,

        [Required]
        [BindRequired]
        bool IsDefault,

        [Required]
        string Label,

        [Required]
        [NotNullOrWhitespace]
        int UserId
    );
}