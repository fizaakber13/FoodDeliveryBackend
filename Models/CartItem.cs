using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        
        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public Restaurant Restaurant { get; set; } = null!;

        [JsonIgnore]
        public MenuItem MenuItem { get; set; } = null!;
    }
}
