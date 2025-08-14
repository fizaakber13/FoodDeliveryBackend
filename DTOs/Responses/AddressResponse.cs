namespace FoodDeliveryBackend.DTOs.Responses
{
    public record AddressResponse(int Id, string Line, bool IsDefault, string Label, int UserId);
}