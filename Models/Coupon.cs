using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("Coupon")]
    public class Coupon
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = "";

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }

        [Required]
        public string DiscountType { get; set; } = "";

        [Required]
        public string Condition { get; set; } = "";

        public decimal? MinOrderAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<RestaurantCoupon> RestaurantCoupons { get; set; } = new List<RestaurantCoupon>();
    }
}
