using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("Coupon")]
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = ""; 
        public string Condition { get; set; } = ""; 
        public decimal? MinOrderAmount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<RestaurantCoupon> RestaurantCoupons { get; set; } = new List<RestaurantCoupon>();
    }
}
