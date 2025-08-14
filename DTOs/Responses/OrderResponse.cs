using System;
using System.Collections.Generic;

namespace FoodDeliveryBackend.DTOs.Responses
{
    public record OrderResponse(int Id, int UserId, int RestaurantId, DateTime OrderDate, decimal TotalAmount, string Status, string Address, string PaymentMethod, List<OrderItemResponse> OrderItems);
}