using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("RestaurantCoupon")]
    public class RestaurantCoupon
    {
        public int Id { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        public int CouponId { get; set; }
        public Coupon Coupon { get; set; } = null!;
    }
}
