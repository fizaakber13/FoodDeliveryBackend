using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("Order")]
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "";


        public string Address { get; set; } = "";
        public string PaymentMethod { get; set; } = "";

        public User? User { get; set; }
        public Restaurant? Restaurant { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }

}
