using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("CartItem")]
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }

        
        public User User { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
        public MenuItem MenuItem { get; set; } = null!;
    }
}
