using FoodDeliveryBackend.DTOs.Requests;
using FoodDeliveryBackend.DTOs.Responses;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Services.Interfaces;
using FoodDeliveryBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FoodDeliveryBackend.Services.Implementations
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderItemResponse?> GetOrderItemByIdAsync(int id)
        {
            var orderItem = await _unitOfWork.OrderItems.FirstOrDefaultAsync(oi => oi.Id == id, oi => oi.MenuItem);
            if (orderItem == null) return null;
            return new OrderItemResponse(orderItem.Id, orderItem.MenuItemId, orderItem.Quantity, orderItem.Price, orderItem.MenuItem!.Name);
        }

        public async Task<IEnumerable<OrderItemResponse>> GetAllOrderItemsAsync()
        {
            var orderItems = await _unitOfWork.OrderItems.GetAsync(null, oi => oi.MenuItem);
            return orderItems.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name));
        }

        public async Task<OrderItemResponse> CreateOrderItemAsync(CreateOrderItemRequest orderItemDto)
        {
            var menuItem = (await _unitOfWork.MenuItems.FindAsync(mi => mi.Id == orderItemDto.MenuItemId)).FirstOrDefault();
            if (menuItem == null)
            {
                throw new Exception($"Menu item with ID {orderItemDto.MenuItemId} not found.");
            }
            var orderItem = new OrderItem
            {
                MenuItemId = orderItemDto.MenuItemId,
                Quantity = orderItemDto.Quantity,
                Price = (decimal)menuItem.Price // Assuming price is fetched from MenuItem
            };
            await _unitOfWork.OrderItems.AddAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();
            return new OrderItemResponse(orderItem.Id, orderItem.MenuItemId, orderItem.Quantity, orderItem.Price, menuItem.Name);
        }

        public async Task UpdateOrderItemAsync(int id, OrderItemResponse orderItemDto)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null) return;

            orderItem.MenuItemId = orderItemDto.MenuItemId;
            orderItem.Quantity = orderItemDto.Quantity;
            orderItem.Price = orderItemDto.UnitPrice;

            _unitOfWork.OrderItems.Update(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null) return;

            _unitOfWork.OrderItems.Remove(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderItemResponse>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderItems = await _unitOfWork.OrderItems.GetAsync(oi => oi.OrderId == orderId, oi => oi.MenuItem);
            return orderItems.Select(oi => new OrderItemResponse(oi.Id, oi.MenuItemId, oi.Quantity, oi.Price, oi.MenuItem!.Name));
        }
    }
}