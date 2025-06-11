using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("MenuItem")]
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public double Price { get; set; }

        
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
    }
}
