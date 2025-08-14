namespace FoodDeliveryBackend.DTOs.Responses
{
    public record RestaurantResponse(int Id, string Name, string Email, string Cuisine, string Address, double Rating, string Contact);
}