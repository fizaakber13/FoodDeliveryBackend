using System.ComponentModel.DataAnnotations;
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

        [Required]
        public string Name { get; set; } = "";

        
        [Required]
        [RegularExpression(@"^(\d{10}|[\w\.-]+@[\w\.-]+\.\w{2,})$",
            ErrorMessage = "Must be a 10-digit number or a valid email address.")]
        public string EmailOrPhone { get; set; } = "";

        
        public bool IsAdmin { get; set; } = false;

        public ICollection<Address> Addresses { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
