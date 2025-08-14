using System;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.DTOs.Requests
{
    public record CreateCouponRequest(
        [Required]
        [StringLength(50)]
        string Code,

        [Range(0, double.MaxValue)]
        decimal DiscountAmount,

        [Required]
        [StringLength(50)]
        string DiscountType,

        [Required]
        [StringLength(255)]
        string Condition,

        [Range(0, double.MaxValue)]
        decimal? MinOrderAmount,

        [DataType(DataType.Date)]
        DateTime? ExpirationDate,

        bool IsActive
    );
}