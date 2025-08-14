using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemResponse?> GetOrderItemByIdAsync(int id);
        Task<IEnumerable<OrderItemResponse>> GetAllOrderItemsAsync();
        Task<OrderItemResponse> CreateOrderItemAsync(CreateOrderItemRequest orderItemDto);
        Task UpdateOrderItemAsync(int id, OrderItemResponse orderItemDto);
        Task DeleteOrderItemAsync(int id);
        Task<IEnumerable<OrderItemResponse>> GetOrderItemsByOrderIdAsync(int orderId);
    }
}
