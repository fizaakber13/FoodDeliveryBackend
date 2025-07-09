using System;

namespace FoodDeliveryBackend.Models
{
    public class OtpRequest
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string OtpCode { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
