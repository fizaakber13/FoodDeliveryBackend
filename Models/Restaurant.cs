using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("Restaurant")]
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Cuisine { get; set; } = "";
        public string Address { get; set; } = "";

        
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<RestaurantCoupon> RestaurantCoupons { get; set; } = new List<RestaurantCoupon>();
    }
}
