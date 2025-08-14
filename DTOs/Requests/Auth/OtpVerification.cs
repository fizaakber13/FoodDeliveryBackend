namespace FoodDeliveryBackend.DTOs.Requests.Auth
{
    public class OtpVerification
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
        public string? Name { get; set; }
    }
}
