using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace FoodDeliveryBackend.Models
{
    [Table("User")]
    public class User
    {
        public User()
        {
            Addresses = new List<Address>();
            CartItems = new List<CartItem>();
            Orders = new List<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string EmailOrPhone { get; set; } = "";

        // Relationships
        public ICollection<Address> Addresses { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
