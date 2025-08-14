namespace FoodDeliveryBackend.DTOs.Responses
{
    public record MenuItemResponse(int Id, string Name, string Category, string Description, double Price, double Rating, int RestaurantId);
}