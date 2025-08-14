using System.Collections.Generic;

namespace FoodDeliveryBackend.DTOs.Responses
{
    public record CartSummaryResponse(int RestaurantId, string RestaurantName, decimal TotalPrice, List<CartSummaryItemResponse> Items);

    public record CartSummaryItemResponse(int Id, int MenuItemId, string Name, int Quantity, decimal UnitPrice, decimal SubTotal);
}