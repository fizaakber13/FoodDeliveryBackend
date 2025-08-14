namespace FoodDeliveryBackend.DTOs.Responses.Auth
{
    public record TokenResponse
    (
        string AccessToken,

        string RefreshToken
    );
}
