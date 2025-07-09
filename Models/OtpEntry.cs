using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.Models
{
    public class OtpEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }

        public DateTime CreatedAt { get; set; }  
    }
}
