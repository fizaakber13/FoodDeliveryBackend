using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("Address")]
    public class Address
    {
        public int Id { get; set; }
        public string Line { get; set; } = "";
        public bool IsDefault { get; set; }

       
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
