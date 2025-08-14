namespace FoodDeliveryBackend.DTOs.Responses
{
    public record CartTotalResponse(decimal Total, decimal Discount, decimal FinalAmount, string Message);
}