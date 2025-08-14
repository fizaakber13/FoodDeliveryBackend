using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateOrderRequest(
        [Required]
        int UserId,
        [Required]
        int RestaurantId,
        [Required]
        int AddressId, // Changed from string Address
        [Required]
        [StringLength(50)]
        string PaymentMethod,
        string? CouponCode, // Added nullable CouponCode
        [Required]
        [MinLength(1, ErrorMessage = "Order must have at least one item.")]
        List<CreateOrderItemRequest> OrderItems
    );
}