using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBackend.Models
{
    public class OtpEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }  
    }
}
