using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

using FoodDeliveryBackend.Pagination;

namespace FoodDeliveryBackend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse?> GetOrderByIdAsync(int id);
        Task<PagedList<OrderResponse>> GetAllOrdersAsync(PaginationParams paginationParams);
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest orderDto);
        Task UpdateOrderAsync(int id, OrderResponse orderDto);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderResponse>> GetOrdersByRestaurantIdAsync(int restaurantId);
        Task UpdateOrderStatusAsync(int id, string status);
        Task<IEnumerable<OrderResponse>> SearchOrdersByRestaurantAsync(int userId, string restaurantName);
    }
}
