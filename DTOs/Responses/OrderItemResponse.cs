namespace FoodDeliveryBackend.DTOs.Responses
{
    public record OrderItemResponse(int Id, int MenuItemId, int Quantity, decimal UnitPrice, string MenuItemName);
}