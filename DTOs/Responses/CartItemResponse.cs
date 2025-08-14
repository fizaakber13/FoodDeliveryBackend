namespace FoodDeliveryBackend.DTOs.Responses
{
    public record CartItemResponse(int Id, int UserId, int RestaurantId, int MenuItemId, int Quantity);
}