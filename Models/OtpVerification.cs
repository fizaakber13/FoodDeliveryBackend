using System.ComponentModel.DataAnnotations;

public class OtpVerification
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Otp { get; set; } = "";

    
}
