using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryBackend.Models
{
    [Table("Address")]
    public class Address
    {
        public int Id { get; set; }

        public string Line { get; set; } = "";

        public bool IsDefault { get; set; }

        public string Label { get; set; } = "Home";

        public int UserId { get; set; }

        public User? User { get; set; }

        
        [NotMapped]
        public string FullAddress
        {
            get
            {
                return $"{(IsDefault ? "[Default] " : "")}{Label}: {Line}";
            }
        }
    }
}
